using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FreschOne.Controllers
{
    public class ProcessEventsController : BaseController
    {
        public ProcessEventsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }


        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult CreateStep(int processId, int? stepId, int? processInstanceId, int userId)
        {
            SetUserAccess(userId);
            ViewBag.userid = userId;
            ViewBag.processInstanceId = processInstanceId;

            var columnCalcsParsed = new Dictionary<string, List<(string Function, string Column)>>();

            if (stepId is null)
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"SELECT TOP 1 ID FROM foProcessSteps 
                                  WHERE ProcessID = @ProcessID AND Active = 1 
                                  ORDER BY StepNo", conn);
                    cmd.Parameters.AddWithValue("@ProcessID", processId);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        return NotFound("No steps found for the selected process.");
                    stepId = Convert.ToInt32(result);
                }
            }

            string stepDescription = "";
            double stepNo = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                var detailCmd = new SqlCommand("SELECT StepDescription, StepNo FROM foProcessSteps WHERE ID = @StepID", conn);
                detailCmd.Parameters.AddWithValue("@StepID", stepId);

                using (var reader = detailCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stepDescription = reader["StepDescription"].ToString();
                        stepNo = Convert.ToDouble(reader["StepNo"]);
                    }
                }
            }

            ViewBag.StepDescription = stepDescription;
            ViewBag.StepNo = stepNo;
            ViewBag.stepId = stepId;

            var tables = new List<foProcessDetail>();
            var tableData = new Dictionary<string, List<Dictionary<string, object>>>();
            var tablePrefixes = GetTablePrefixes();
            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            var columnTypes = new Dictionary<string, Dictionary<string, string>>();
            var columnLengths = new Dictionary<string, Dictionary<string, int>>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var ignoredColumns = GetIgnoredColumns(conn);

                var query = @"SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription, ColumnCalcs
                      FROM foProcessDetail 
                      WHERE StepID = @StepID AND Active = 1
                      ORDER BY Parent DESC, ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StepID", stepId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new foProcessDetail
                            {
                                TableName = reader["TableName"].ToString(),
                                ColumnQuery = reader["ColumnQuery"].ToString(),
                                FormType = reader["FormType"].ToString(),
                                ColumnCount = reader["ColumnCount"] != DBNull.Value ? Convert.ToInt32(reader["ColumnCount"]) : (int?)null,
                                Parent = reader["Parent"] != DBNull.Value && Convert.ToBoolean(reader["Parent"]),
                                FKColumn = reader["FKColumn"].ToString(),
                                TableDescription = reader["TableDescription"].ToString(),
                                ColumnCalcs = reader["ColumnCalcs"].ToString(),
                            });
                        }
                    }
                }

                foreach (var table in tables)
                {
                    if (!string.IsNullOrEmpty(table.FKColumn))
                        ignoredColumns.Add(table.FKColumn);

                    // Fetch foreign keys (maintenance + non-maintenance)
                    foreignKeys[table.TableName] = GetForeignKeyColumns(table.TableName);

                    // Fetch foreign key options for each FK table
                    foreach (var fk in foreignKeys[table.TableName])
                    {
                        if (!foreignKeyOptions.ContainsKey(fk.TableName))
                        {
                            foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                        }
                    }

                    // Parse column calcs
                    var calcs = new List<(string Function, string Column)>();
                    if (!string.IsNullOrWhiteSpace(table.ColumnCalcs))
                    {
                        var expressions = table.ColumnCalcs.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var expr in expressions)
                        {
                            var match = Regex.Match(expr.Trim(), @"(Count|Sum)\(([^)]+)\)", RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                calcs.Add((match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim()));
                            }
                        }
                    }
                    columnCalcsParsed[table.TableName] = calcs;

                    // Get columns for the table
                    List<string> columns = table.ColumnQuery.Trim() == "*"
                        ? GetAllColumnsForTable(table.TableName, conn).Where(col => !ignoredColumns.Contains(col)).ToList()
                        : table.ColumnQuery.Split(',').Select(col => col.Trim()).Where(col => !ignoredColumns.Contains(col)).ToList();

                    // Get column types and lengths
                    columnTypes[table.TableName] = new Dictionary<string, string>();
                    columnLengths[table.TableName] = new Dictionary<string, int>();

                    var metaQuery = @"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                              FROM INFORMATION_SCHEMA.COLUMNS 
                              WHERE TABLE_NAME = @TableName";

                    using (var metaCmd = new SqlCommand(metaQuery, conn))
                    {
                        metaCmd.Parameters.AddWithValue("@TableName", table.TableName);
                        using (var metaReader = metaCmd.ExecuteReader())
                        {
                            while (metaReader.Read())
                            {
                                string col = metaReader["COLUMN_NAME"].ToString();
                                string type = metaReader["DATA_TYPE"].ToString();
                                int length = metaReader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value
                                    ? Convert.ToInt32(metaReader["CHARACTER_MAXIMUM_LENGTH"])
                                    : 0;

                                columnTypes[table.TableName][col] = type;
                                columnLengths[table.TableName][col] = length;
                            }
                        }
                    }

                    var row = columns.ToDictionary(col => col, col => (object)null);
                    tableData[table.TableName] = new List<Dictionary<string, object>> { row };

                    var model = new DynamicTableViewModel
                    {
                        TableName = table.TableName,
                        Columns = columns,
                        RowCount = 1,
                        StartIndex = 0,
                        ColumnTypes = columnTypes[table.TableName],
                        ColumnLengths = columnLengths[table.TableName],
                        ForeignKeys = foreignKeys[table.TableName],
                        ForeignKeyOptions = foreignKeyOptions
                    };

                    ViewBag.InitialModels ??= new List<DynamicTableViewModel>();
                    (ViewBag.InitialModels as List<DynamicTableViewModel>).Add(model);
                }
            }

            var fkTables = new Dictionary<string, string>();
            foreach (var fkPair in foreignKeys)
            {
                foreach (var fk in fkPair.Value)
                {
                    fkTables[fk.ColumnName] = fk.TableName;
                }
            }
            ViewBag.ForeignKeyTables = fkTables;


            ViewBag.CalculationConfig = columnCalcsParsed;
            ViewBag.ReportTables = tables;
            ViewBag.ReportData = tableData;
            ViewBag.TableDescriptions = tables.ToDictionary(t => t.TableName, t => CleanTableName(t.TableName, tablePrefixes));
            ViewBag.ForeignKeys = foreignKeys;
            ViewBag.ForeignKeyOptions = foreignKeyOptions;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;
            ViewBag.SearchResult = true;
            ViewBag.UserList = GetApproverSelectList();

            return View();
        }

        public IActionResult PendingStep(int Eventid, int processId, int? stepId, int? processInstanceId, int userId)
        {
            SetUserAccess(userId);
            ViewBag.userid = userId;
            ViewBag.Eventid = Eventid;
            ViewBag.processInstanceId = processInstanceId;

            var columnCalcsParsed = new Dictionary<string, List<(string Function, string Column)>>();

            if (stepId is null)
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
                SELECT TOP 1 ID 
                FROM foProcessSteps 
                WHERE ProcessID = @ProcessID AND Active = 1 
                ORDER BY StepNo", conn);
                    cmd.Parameters.AddWithValue("@ProcessID", processId);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        return NotFound("No steps found for the selected process.");
                    stepId = Convert.ToInt32(result);
                }
            }

            string stepDescription = "";
            double stepNo = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                var detailCmd = new SqlCommand("SELECT StepDescription, StepNo FROM foProcessSteps WHERE ID = @StepID", conn);
                detailCmd.Parameters.AddWithValue("@StepID", stepId);
                using (var reader = detailCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stepDescription = reader["StepDescription"].ToString();
                        stepNo = Convert.ToDouble(reader["StepNo"]);
                    }
                }
            }

            ViewBag.StepDescription = stepDescription;
            ViewBag.StepNo = stepNo;
            ViewBag.stepId = stepId;

            var tables = new List<foProcessDetail>();
            var tableData = new Dictionary<string, List<Dictionary<string, object>>>();
            var prefilledValues = new Dictionary<string, List<Dictionary<string, object>>>();
            var tablePrefixes = GetTablePrefixes();
            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            var columnTypes = new Dictionary<string, Dictionary<string, string>>();
            var columnLengths = new Dictionary<string, Dictionary<string, int>>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var ignoredColumns = GetIgnoredColumns(conn);

                var query = @"
            SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription, ColumnCalcs
            FROM foProcessDetail 
            WHERE StepID = @StepID AND Active = 1
            ORDER BY Parent DESC, ID";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StepID", stepId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new foProcessDetail
                            {
                                TableName = reader["TableName"].ToString(),
                                ColumnQuery = reader["ColumnQuery"].ToString(),
                                FormType = reader["FormType"].ToString(),
                                ColumnCount = reader["ColumnCount"] != DBNull.Value ? Convert.ToInt32(reader["ColumnCount"]) : (int?)null,
                                Parent = reader["Parent"] != DBNull.Value && Convert.ToBoolean(reader["Parent"]),
                                FKColumn = reader["FKColumn"]?.ToString(),
                                TableDescription = reader["TableDescription"].ToString(),
                                ColumnCalcs = reader["ColumnCalcs"]?.ToString(),
                            });
                        }
                    }
                }

                foreach (var table in tables)
                {
                    var calcs = new List<(string Function, string Column)>();
                    if (!string.IsNullOrWhiteSpace(table.ColumnCalcs))
                    {
                        var expressions = table.ColumnCalcs.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var expr in expressions)
                        {
                            var match = Regex.Match(expr.Trim(), @"(Count|Sum)\(([^)]+)\)", RegexOptions.IgnoreCase);
                            if (match.Success)
                            {
                                calcs.Add((match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim()));
                            }
                        }
                    }
                    columnCalcsParsed[table.TableName] = calcs;
                }

                ViewBag.CalculationConfig = columnCalcsParsed;

                foreach (var table in tables)
                {
                    if (!string.IsNullOrEmpty(table.FKColumn))
                        ignoredColumns.Add(table.FKColumn);

                    var columns = table.ColumnQuery.Trim() == "*"
                        ? GetAllColumnsForTable(table.TableName, conn).Where(col => !ignoredColumns.Contains(col)).ToList()
                        : table.ColumnQuery.Split(',').Select(c => c.Trim()).Where(col => !ignoredColumns.Contains(col)).ToList();

                    columnTypes[table.TableName] = new Dictionary<string, string>();
                    columnLengths[table.TableName] = new Dictionary<string, int>();

                    using (var metaCmd = new SqlCommand(@"
                SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @TableName", conn))
                    {
                        metaCmd.Parameters.AddWithValue("@TableName", table.TableName);
                        using (var metaReader = metaCmd.ExecuteReader())
                        {
                            while (metaReader.Read())
                            {
                                string col = metaReader["COLUMN_NAME"].ToString();
                                string type = metaReader["DATA_TYPE"].ToString();
                                int length = metaReader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value ? Convert.ToInt32(metaReader["CHARACTER_MAXIMUM_LENGTH"]) : 0;

                                columnTypes[table.TableName][col] = type;
                                columnLengths[table.TableName][col] = length;
                            }
                        }
                    }

                    foreignKeys[table.TableName] = GetForeignKeyColumns(table.TableName).ToList();
                    foreach (var fk in foreignKeys[table.TableName])
                    {
                        if (!foreignKeyOptions.ContainsKey(fk.TableName))
                        {
                            foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                        }
                    }

                    var rows = new List<Dictionary<string, object>>();

                    if (table.FormType == "F")
                    {
                        var latestCmd = new SqlCommand(@"
                    SELECT ProcessEventID, RecordID, DataSetUpdate
                    FROM foProcessEventsDetail
                    WHERE ProcessInstanceID = @InstanceID 
                      AND TableName = @TableName 
                      AND Active = 1 ORDER BY 1 DESC", conn);
                        latestCmd.Parameters.AddWithValue("@InstanceID", processInstanceId);
                        latestCmd.Parameters.AddWithValue("@TableName", table.TableName);

                        using (var reader = latestCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var json = reader["DataSetUpdate"].ToString();
                                var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                var filtered = columns.ToDictionary(c => c, c => raw.ContainsKey(c) ? raw[c] : null);

                                filtered["RecordID"] = reader["RecordID"]?.ToString();
                                ReplaceFKValues(filtered, foreignKeys[table.TableName], foreignKeyOptions);
                                rows.Add(filtered);
                            }
                        }
                    }
                    else if (table.FormType == "T")
                    {
                        var allRowsCmd = new SqlCommand(@"
                    SELECT a.ProcessEventID, a.RecordID, a.DataSetUpdate
                    FROM foProcessEventsDetail a
                    INNER JOIN (
                        SELECT MAX(ID) AS MaxID
                        FROM foProcessEventsDetail
                        WHERE ProcessInstanceID = @InstanceID
                          AND StepID = @StepID
                          AND TableName = @TableName
                          AND Active = 1
                        GROUP BY RecordID
                    ) b ON a.ID = b.MaxID
                    ORDER BY a.ID", conn);

                        allRowsCmd.Parameters.AddWithValue("@InstanceID", processInstanceId);
                        allRowsCmd.Parameters.AddWithValue("@StepID", stepId);
                        allRowsCmd.Parameters.AddWithValue("@TableName", table.TableName);

                        using (var reader = allRowsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var json = reader["DataSetUpdate"].ToString();
                                var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                var filtered = columns.ToDictionary(c => c, c => raw.ContainsKey(c) ? raw[c] : null);

                                filtered["RecordID"] = reader["RecordID"]?.ToString();
                                ReplaceFKValues(filtered, foreignKeys[table.TableName], foreignKeyOptions);
                                rows.Add(filtered);
                            }
                        }
                    }

                    if (!rows.Any())
                    {
                        var emptyRow = columns.ToDictionary(c => c, c => (object)null);
                        emptyRow["RecordID"] = null;
                        rows.Add(emptyRow);
                    }

                    tableData[table.TableName] = rows;
                    prefilledValues[table.TableName] = rows;
                }

                ViewBag.PrefilledValues = prefilledValues;
                PopulateProcessHistory(conn, null, processInstanceId);
            }

            // Rework Comment
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
            SELECT TOP 1 d.DataSetUpdate, u.FirstName, u.LastName
            FROM foApprovalEventsDetail d
            INNER JOIN foUsers u ON d.CreatedUserID = u.ID
            WHERE d.ProcessInstanceID = @InstanceID
            ORDER BY d.ID DESC", conn);

                cmd.Parameters.AddWithValue("@InstanceID", processInstanceId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader["DataSetUpdate"].ToString());
                        if (data.ContainsKey("Comment") && data.ContainsKey("Decision") && data["Decision"].ToString() == "Rework")
                        {
                            ViewBag.ReworkComment = data["Comment"]?.ToString();
                            ViewBag.ReworkBy = $"{reader["FirstName"]} {reader["LastName"]}";
                        }
                    }
                }
            }

            // Ad-hoc Approvers
            var originalAdhocApprovers = new List<string>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var approverCmd = new SqlCommand(@"
            SELECT DISTINCT u.FirstName, u.LastName
            FROM foApprovalEvents e
            INNER JOIN foUsers u ON e.UserID = u.ID
            WHERE e.ProcessInstanceID = @InstanceID
              AND e.StepID = 0
              AND e.Active = 1", conn);

                approverCmd.Parameters.AddWithValue("@InstanceID", processInstanceId);

                using (var reader = approverCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        originalAdhocApprovers.Add($"{reader["FirstName"]} {reader["LastName"]}");
                    }
                }
            }

            var fkTables = new Dictionary<string, string>();
            foreach (var fkPair in foreignKeys)
            {
                foreach (var fk in fkPair.Value)
                {
                    fkTables[fk.ColumnName] = fk.TableName;
                }
            }
            ViewBag.ForeignKeyTables = fkTables;

            ViewBag.OriginalAdhocApprovers = originalAdhocApprovers;
            ViewBag.ReportTables = tables;
            ViewBag.ReportData = tableData;
            ViewBag.TableDescriptions = tables.ToDictionary(t => t.TableName, t => CleanTableName(t.TableName, tablePrefixes));
            ViewBag.ForeignKeys = foreignKeys;
            ViewBag.ForeignKeyOptions = foreignKeyOptions;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;
            ViewBag.SearchResult = true;

            return View();
        }

        // ReplaceFKValues Helper
        public void ReplaceFKValues(Dictionary<string, object> row, List<ForeignKeyInfo> fkInfos, Dictionary<string, List<SelectListItem>> fkOptions)
        {
            foreach (var fk in fkInfos)
            {
                var columnName = fk.ColumnName;
                if (row.ContainsKey(columnName) && row[columnName] != null)
                {
                    var idValue = row[columnName].ToString();
                    var options = fkOptions.ContainsKey(fk.TableName) ? fkOptions[fk.TableName] : new List<SelectListItem>();
                    var match = options.FirstOrDefault(x => x.Value == idValue);
                    if (match != null)
                    {
                        // Don't overwrite the FK value!
                        // Optionally: Add a new key like "{ColumnName}_Display" if needed for showing description
                        row[$"{columnName}_Display"] = match.Text;
                    }
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNewStepData(IFormCollection form, int userId, int stepId, int? processInstanceId, string action, bool SendForApproval = false, List<int> SelectedApproverIds = null)
        {
            var insertedIds = new Dictionary<string, int>();
            var rowErrors = new List<string>();
            var baseAttachmentFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            Directory.CreateDirectory(baseAttachmentFolder);

            // Temp list to handle attachment saving AFTER insert
            var attachmentsToProcess = new List<(string TableName, string ColName, string RowIndex, IFormFile File, string Desc)>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Load table metadata
                        var tables = new List<foProcessDetail>();
                        var cmd = new SqlCommand(@"SELECT TableName, FormType, FKColumn, TableDescription 
                                           FROM foProcessDetail 
                                           WHERE StepID = @StepID AND Active = 1", conn, transaction);
                        cmd.Parameters.AddWithValue("@StepID", stepId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(new foProcessDetail
                                {
                                    TableName = reader["TableName"].ToString(),
                                    FormType = reader["FormType"].ToString(),
                                    FKColumn = reader["FKColumn"].ToString(),
                                    TableDescription = reader["TableDescription"].ToString()
                                });
                            }
                        }

                        // Step description
                        string stepDescription = "";
                        using (var descCmd = new SqlCommand("SELECT StepDescription FROM foProcessSteps WHERE ID = @StepID", conn, transaction))
                        {
                            descCmd.Parameters.AddWithValue("@StepID", stepId);
                            stepDescription = descCmd.ExecuteScalar()?.ToString();
                        }

                        // Generate ProcessInstanceID
                        if (!processInstanceId.HasValue || processInstanceId == 0)
                        {
                            int? maxProcessInstanceId = null;
                            foreach (var table in new[] { "foProcessEvents", "foProcessEventsArchive" })
                            {
                                using var maxCmd = new SqlCommand($"SELECT MAX(ProcessInstanceID) FROM {table}", conn, transaction);
                                var result = maxCmd.ExecuteScalar();
                                if (result != DBNull.Value && result != null)
                                {
                                    var id = Convert.ToInt32(result);
                                    if (!maxProcessInstanceId.HasValue || id > maxProcessInstanceId.Value)
                                        maxProcessInstanceId = id;
                                }
                            }
                            processInstanceId = (maxProcessInstanceId ?? 0) + 1;
                        }

                        // Insert initial process event
                        int firstEventId;
                        using (var insertEventCmd = new SqlCommand(@"
                    INSERT INTO foProcessEvents (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, DateCompleted, Active)
                    OUTPUT INSERTED.ID 
                    VALUES (@ProcessInstanceID, @StepID, 0, @UserID, GETDATE(), GETDATE(), 1)", conn, transaction))
                        {
                            insertEventCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            insertEventCmd.Parameters.AddWithValue("@StepID", stepId);
                            insertEventCmd.Parameters.AddWithValue("@UserID", userId);
                            firstEventId = Convert.ToInt32(insertEventCmd.ExecuteScalar());
                        }

                        // Insert rows per table
                        foreach (var table in tables)
                        {
                            string tableName = table.TableName;
                            string fkColumn = table.FKColumn;
                            var columnDefs = GetColumnDefinitions(tableName, conn, transaction).Select(cd => cd.ColumnName).ToList();

                            var rows = form.Keys
                                .Where(k => k.StartsWith(tableName + "_"))
                                .GroupBy(k => Regex.Match(k, @"_(\d+)$").Success ? Regex.Match(k, @"_(\d+)$").Groups[1].Value : "0")
                                .ToList();

                            foreach (var rowGroup in rows)
                            {
                                var rowIndex = rowGroup.Key;
                                try
                                {
                                    var fields = rowGroup.ToDictionary(k => k, k => form[k]);
                                    var columnsSqlList = new List<string>();
                                    var parametersSqlList = new List<string>();
                                    var parameterValues = new Dictionary<string, object>();

                                    foreach (var field in fields)
                                    {
                                        var rawColName = field.Key.Substring(tableName.Length + 1);
                                        var colName = Regex.Replace(rawColName, "_\\d+$", "");
                                        var value = field.Value.ToString();

                                        if (!columnDefs.Contains(colName, StringComparer.OrdinalIgnoreCase)) continue;

                                        if (colName.StartsWith("attachment_"))
                                        {
                                            bool isTabular = table.FormType == "T";
                                            string descKey = isTabular ? $"desc_{tableName}_{colName}_{rowIndex}" : $"desc_{tableName}_{colName}";
                                            string fileKey = isTabular ? $"file_{tableName}_{colName}_{rowIndex}" : $"file_{tableName}_{colName}";
                                            var file = form.Files.FirstOrDefault(f => f.Name == fileKey);
                                            string desc = form.TryGetValue(descKey, out var d) ? d.ToString().Trim() : "";

                                            if (file != null && !string.IsNullOrWhiteSpace(file.FileName))
                                            {
                                                var blockedExtensions = new[] { ".exe", ".bat", ".cmd", ".js", ".vbs", ".msi", ".dll" };
                                                if (blockedExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                                                {
                                                    rowErrors.Add($"Blocked file type: '{file.FileName}' is not allowed for '{tableName}', row {rowIndex}");
                                                    continue;
                                                }

                                                // Delay value assignment — we'll save file & update DB after insert
                                                attachmentsToProcess.Add((tableName, colName, rowIndex, file, desc));
                                                continue; // skip for now
                                            }

                                            // If editing or no new file
                                            string hiddenKey = isTabular ? $"{tableName}_{colName}_{rowIndex}" : $"{tableName}_{colName}";
                                            string existing = form.TryGetValue(hiddenKey, out var h) ? h.ToString() : "";

                                            if (!string.IsNullOrWhiteSpace(existing))
                                                value = $"{(string.IsNullOrWhiteSpace(desc) ? existing.Split(';')[0] : desc)};{existing.Split(';').ElementAtOrDefault(1)}";
                                            else
                                                value = null;

                                            parameterValues[colName] = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
                                        }
                                        else
                                        {
                                            parameterValues[colName] = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
                                        }

                                        columnsSqlList.Add(colName);
                                        parametersSqlList.Add("@" + colName);
                                    }

                                    // FK logic
                                    if (!string.IsNullOrEmpty(fkColumn))
                                    {
                                        var fkBase = fkColumn.Replace("ID", "", StringComparison.OrdinalIgnoreCase).ToLower();
                                        var parentKey = insertedIds.Keys.FirstOrDefault(k =>
                                            k.StartsWith("tbl_tran_", StringComparison.OrdinalIgnoreCase) &&
                                            k.ToLower().Contains(fkBase));

                                        if (!string.IsNullOrEmpty(parentKey) && insertedIds.TryGetValue(parentKey, out int fkValue))
                                        {
                                            parameterValues[fkColumn] = fkValue;
                                            columnsSqlList.Add(fkColumn);
                                            parametersSqlList.Add("@" + fkColumn);
                                        }
                                        else
                                        {
                                            rowErrors.Add($"Unable to resolve FK '{fkColumn}' for '{tableName}'");
                                        }
                                    }

                                    // System fields
                                    parameterValues["Active"] = 1;
                                    parameterValues["CreatedUserID"] = userId;
                                    parameterValues["CreatedDate"] = DateTime.Now;
                                    columnsSqlList.AddRange(new[] { "Active", "CreatedUserID", "CreatedDate" });
                                    parametersSqlList.AddRange(new[] { "@Active", "@CreatedUserID", "@CreatedDate" });

                                    // INSERT
                                    string insertSql = $"INSERT INTO {tableName} ({string.Join(", ", columnsSqlList)}) OUTPUT INSERTED.ID VALUES ({string.Join(", ", parametersSqlList)})";
                                    int newId;
                                    using (var insertCmd = new SqlCommand(insertSql, conn, transaction))
                                    {
                                        foreach (var col in columnsSqlList)
                                            insertCmd.Parameters.AddWithValue("@" + col, parameterValues[col] ?? DBNull.Value);

                                        newId = Convert.ToInt32(insertCmd.ExecuteScalar());
                                        insertedIds[$"{tableName}_{rowIndex}"] = newId;
                                    }

                                    // Save attachments for this row
                                    // 🗃️ Now process attachments that were deferred
                                    foreach (var att in attachmentsToProcess.Where(a => a.TableName == tableName && a.RowIndex == rowIndex))
                                    {
                                        var file = att.File;
                                        if (file != null && file.Length > 0)
                                        {
                                            var safeDesc = string.IsNullOrWhiteSpace(att.Desc) ? "No Description" : att.Desc;
                                            var targetFolder = Path.Combine(baseAttachmentFolder, tableName, newId.ToString());
                                            Directory.CreateDirectory(targetFolder);

                                            var safeFileName = Path.GetFileName(file.FileName);
                                            var fullPath = Path.Combine(targetFolder, safeFileName);
                                            var virtualPath = Path.Combine("Attachments", tableName, newId.ToString(), safeFileName);

                                            // 🧠 Save file
                                            using (var fs = new FileStream(fullPath, FileMode.Create))
                                            {
                                                file.CopyTo(fs);
                                            }

                                            var finalValue = $"{safeDesc};{virtualPath.Replace("\\", "/")}";

                                            // 📌 Update the actual DB record
                                            using var updateCmd = new SqlCommand($"UPDATE {tableName} SET {att.ColName} = @value WHERE ID = @recordId", conn, transaction);
                                            updateCmd.Parameters.AddWithValue("@value", finalValue);
                                            updateCmd.Parameters.AddWithValue("@recordId", newId);
                                            updateCmd.ExecuteNonQuery();

                                            // 🔁 Sync value into snapshot so it's included in audit JSON
                                            parameterValues[att.ColName] = finalValue;
                                        }
                                    }


                                    // Audit
                                    var rowSnapshot = new Dictionary<string, object>(parameterValues)
                                    {
                                        ["RecordID"] = newId,
                                        ["StepDescription"] = stepDescription,
                                        ["TableDescription"] = table.TableDescription,
                                        ["interactionType"] = "Insert",
                                        ["CreatedUserID"] = userId,
                                        ["CreatedDate"] = DateTime.Now
                                    };
                                    string json = JsonConvert.SerializeObject(rowSnapshot);

                                    using var auditCmd = new SqlCommand(@"
                                INSERT INTO foProcessEventsDetail 
                                (ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
                                VALUES (@ProcessEventID, @ProcessInstanceID, @StepID, @TableName, @RecordID, @DataSetUpdate, GETDATE(), @CreatedUserID, 1)", conn, transaction);
                                    auditCmd.Parameters.AddWithValue("@ProcessEventID", firstEventId);
                                    auditCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                    auditCmd.Parameters.AddWithValue("@StepID", stepId);
                                    auditCmd.Parameters.AddWithValue("@TableName", tableName);
                                    auditCmd.Parameters.AddWithValue("@RecordID", newId);
                                    auditCmd.Parameters.AddWithValue("@DataSetUpdate", json);
                                    auditCmd.Parameters.AddWithValue("@CreatedUserID", userId);
                                    auditCmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    rowErrors.Add($"Error saving to table '{table.TableName}', row {rowIndex}: {ex.Message}");
                                }
                            }
                        }

                        if (rowErrors.Any())
                        {
                            transaction.Rollback();
                            TempData["RowErrors"] = rowErrors;
                            return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });
                        }

                        HandleNextStep(conn, transaction, stepId, firstEventId, processInstanceId, userId, action, SendForApproval, SelectedApproverIds);

                        // ✅ Fetch next step assignment BEFORE committing the transaction
                        string nextAssignmentMessage = GetNextStepAssignment(conn, transaction, firstEventId,action);

                        // ✅ Now safe to commit
                        transaction.Commit();

                        // Store message in TempData so it can survive redirect
                        TempData["SuccessMessage"] = nextAssignmentMessage;
                        TempData["UserId"] = userId;

                        if (action == "SaveLater")
                        {
                            ViewBag.action = "Step Saved Successully";
                        }
                        else if (action == "CancelProcess")
                        {
                            ViewBag.action = "Process Cancelled Successully";
                        }
                        else if (action == "SaveContinue")
                        {
                            ViewBag.action = "Step Submitted Successully";
                        }

                        return RedirectToAction("StepCompleted", "StepCompleted", new { message = nextAssignmentMessage, userId, actionheader = ViewBag.action });

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["RowErrors"] = new List<string> { ex.Message };
                        TempData["Error"] = "An error occurred while saving step data.";
                        return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePendingStepData(IFormCollection form, int userId, int EventID, int stepId, int processInstanceId, string action, bool SendForApproval = false, List<int> SelectedApproverIds = null)
        {
            var insertedIds = new Dictionary<string, int>();
            var rowErrors = new List<string>();
            var baseAttachmentFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            Directory.CreateDirectory(baseAttachmentFolder);

            var attachmentsToProcess = new List<(string TableName, string ColName, string RowIndex, IFormFile File, string Desc)>();


            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var tables = new List<foProcessDetail>();
                        var cmd = new SqlCommand(@"SELECT TableName, FormType, Parent, FKColumn FROM foProcessDetail WHERE StepID = @StepID AND Active = 1", conn, transaction);
                        cmd.Parameters.AddWithValue("@StepID", stepId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(new foProcessDetail
                                {
                                    TableName = reader["TableName"].ToString(),
                                    FormType = reader["FormType"].ToString(),
                                    Parent = Convert.ToBoolean(reader["Parent"]),
                                    FKColumn = reader["FKColumn"]?.ToString()
                                });
                            }
                        }

                        string stepDescription = "";
                        using (var descCmd = new SqlCommand("SELECT StepDescription FROM foProcessSteps WHERE ID = @StepID", conn, transaction))
                        {
                            descCmd.Parameters.AddWithValue("@StepID", stepId);
                            var result = descCmd.ExecuteScalar();
                            if (result != null) stepDescription = result.ToString();
                        }

                        //int processEventId = id;
                        int processEventId;
                        using (var getEventCmd = new SqlCommand(@"
                  SELECT TOP 1 ID FROM foProcessEvents 
                  WHERE ProcessInstanceID = @ProcessInstanceID AND StepID = @StepID AND DateCompleted IS NULL 
                  ORDER BY ID DESC", conn, transaction))
                        {
                            getEventCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            getEventCmd.Parameters.AddWithValue("@StepID", stepId);
                            processEventId = Convert.ToInt32(getEventCmd.ExecuteScalar());
                        }

                        int? rootParentId = null;
                        string rootParentQuery = @"
                    SELECT TOP 1 RecordID FROM foProcessEventsDetail 
                    WHERE ProcessInstanceID = @ProcessInstanceID 
                    AND StepID = (SELECT MIN(StepID) FROM foProcessEvents WHERE ProcessInstanceID = @ProcessInstanceID)
                    AND TableName IN (SELECT TableName FROM foProcessDetail WHERE Parent = 1 AND FKColumn IS NULL) ";
                        using (var rootCmd = new SqlCommand(rootParentQuery, conn, transaction))
                        {
                            rootCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            var result = rootCmd.ExecuteScalar();
                            if (result != null) rootParentId = Convert.ToInt32(result);
                        }

                        var stepParent = tables.FirstOrDefault(t => t.Parent && !string.IsNullOrEmpty(t.FKColumn));
                        var stepParentTable = stepParent?.TableName;

                        foreach (var table in tables.OrderBy(t => t.Parent ? 0 : 1))
                        {
                            string tableName = table.TableName;
                            string formType = table.FormType;
                            string fkColumn = table.FKColumn;
                            var columnDefs = GetColumnDefinitions(tableName, conn, transaction).Select(cd => cd.ColumnName).ToList();

                            var rows = form.Keys
                                .Where(k => k.StartsWith(tableName + "_"))
                                .GroupBy(k => Regex.Match(k, @"_(\d+)$").Success ? Regex.Match(k, @"_(\d+)$").Groups[1].Value : "0")
                                .ToList();

                            foreach (var rowGroup in rows)
                            {
                                var rowIndex = rowGroup.Key;
                                try
                                {
                                    var recordIdKey = $"{tableName}_RecordID_{rowIndex}";
                                    int recordId = form.TryGetValue(recordIdKey, out var rid) && int.TryParse(rid, out var parsedRid) ? parsedRid : 0;

                                    var fields = rowGroup.ToDictionary(k => k, k => form[k]);
                                    var columnsSqlList = new List<string>();
                                    var parametersSqlList = new List<string>();
                                    var parameterValues = new Dictionary<string, object>();

                                    foreach (var field in fields)
                                    {
                                        var rawColName = field.Key.Substring(tableName.Length + 1);
                                        var colName = Regex.Replace(rawColName, "_\\d+$", "");
                                        var value = field.Value.ToString();

                                        if (!columnDefs.Contains(colName, StringComparer.OrdinalIgnoreCase)) continue;

                                        if (colName.StartsWith("attachment_"))
                                        {
                                            bool isTabular = table.FormType == "T";
                                            string descKey = isTabular ? $"desc_{tableName}_{colName}_{rowIndex}" : $"desc_{tableName}_{colName}";
                                            string fileKey = isTabular ? $"file_{tableName}_{colName}_{rowIndex}" : $"file_{tableName}_{colName}";
                                            string hiddenKey = isTabular ? $"{tableName}_{colName}_{rowIndex}" : $"{tableName}_{colName}";

                                            string desc = form.TryGetValue(descKey, out var d) ? d.ToString().Trim() : "";
                                            var file = form.Files.FirstOrDefault(f => f.Name == fileKey);
                                            string existing = form.TryGetValue(hiddenKey, out var h) ? h.ToString() : "";

                                            if (file != null && file.Length > 0)
                                            {
                                                var blocked = new[] { ".exe", ".bat", ".cmd", ".js", ".vbs", ".msi", ".dll" };
                                                if (blocked.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                                                {
                                                    rowErrors.Add($"Blocked file type: '{file.FileName}' is not allowed for '{tableName}', row {rowIndex}");
                                                    continue;
                                                }

                                                attachmentsToProcess.Add((tableName, colName, rowIndex, file, desc));
                                                value = null; // Will set after insert/update
                                            }
                                            else if (!string.IsNullOrWhiteSpace(existing))
                                            {
                                                value = $"{(string.IsNullOrWhiteSpace(desc) ? existing.Split(';')[0] : desc)};{existing.Split(';').ElementAtOrDefault(1)}";
                                            }
                                            else
                                            {
                                                value = null;
                                            }
                                        }


                                        parameterValues[colName] = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;

                                        columnsSqlList.Add(colName);
                                        parametersSqlList.Add("@" + colName);
                                    }

                                    // 🔐 Handle foreign key only if FKColumn is NOT null or empty
                                    if (!string.IsNullOrWhiteSpace(fkColumn))
                                    {
                                        int? fkValue = null;

                                        if (table.Parent && string.IsNullOrEmpty(fkColumn))
                                        {
                                            fkValue = null;
                                        }
                                        else if (!table.Parent && stepParentTable != null && insertedIds.TryGetValue($"{stepParentTable}_{rowIndex}", out int stepParentId))
                                        {
                                            fkValue = stepParentId;
                                        }
                                        else if (rootParentId.HasValue)
                                        {
                                            fkValue = rootParentId.Value;
                                        }

                                        // Only set if value exists
                                        if (fkValue.HasValue)
                                        {
                                            parameterValues[fkColumn] = fkValue.Value;
                                            columnsSqlList.Add(fkColumn);
                                            parametersSqlList.Add("@" + fkColumn);
                                        }
                                    }


                                    parameterValues["Active"] = 1;

                                    if (recordId > 0)
                                    {
                                        parameterValues["ModifiedUserID"] = userId;
                                        parameterValues["ModifiedDate"] = DateTime.Now;

                                        var setClause = columnsSqlList.Select(col => $"{col} = @{col}").ToList();
                                        setClause.Add("ModifiedUserID = @ModifiedUserID");
                                        setClause.Add("ModifiedDate = @ModifiedDate");

                                        var updateSql = $"UPDATE {tableName} SET {string.Join(", ", setClause)} WHERE ID = @RecordID";
                                        using var updateCmd = new SqlCommand(updateSql, conn, transaction);

                                        foreach (var kvp in parameterValues)
                                            updateCmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);

                                        updateCmd.Parameters.AddWithValue("@RecordID", recordId);

                                        updateCmd.ExecuteNonQuery();
                                    }

                                    else
                                    {
                                        // 🔥 INSERT new record if no RecordID
                                        parameterValues["CreatedUserID"] = userId;
                                        parameterValues["CreatedDate"] = DateTime.Now;

                                        columnsSqlList.AddRange(new[] { "CreatedUserID", "CreatedDate" });
                                        parametersSqlList.AddRange(new[] { "@CreatedUserID", "@CreatedDate" });

                                        string insertSql = $"INSERT INTO {tableName} ({string.Join(", ", columnsSqlList)}) OUTPUT INSERTED.ID VALUES ({string.Join(", ", parametersSqlList)})";
                                        using var insertCmd = new SqlCommand(insertSql, conn, transaction);

                                        foreach (var kvp in parameterValues)
                                            insertCmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value);

                                        recordId = Convert.ToInt32(insertCmd.ExecuteScalar());
                                    }

                                    foreach (var att in attachmentsToProcess.Where(a => a.TableName == tableName && a.RowIndex == rowIndex))
                                    {
                                        var file = att.File;
                                        if (file != null && file.Length > 0)
                                        {
                                            var safeDesc = string.IsNullOrWhiteSpace(att.Desc) ? "No Description" : att.Desc;
                                            var targetFolder = Path.Combine(baseAttachmentFolder, tableName, recordId.ToString());
                                            Directory.CreateDirectory(targetFolder);

                                            var safeFileName = Path.GetFileName(file.FileName);
                                            var fullPath = Path.Combine(targetFolder, safeFileName);
                                            var virtualPath = Path.Combine("Attachments", tableName, recordId.ToString(), safeFileName);

                                            using (var fs = new FileStream(fullPath, FileMode.Create))
                                            {
                                                file.CopyTo(fs);
                                            }

                                            var finalValue = $"{safeDesc};{virtualPath.Replace("\\", "/")}";

                                            using var updateCmd = new SqlCommand($"UPDATE {tableName} SET {att.ColName} = @value WHERE ID = @recordId", conn, transaction);
                                            updateCmd.Parameters.AddWithValue("@value", finalValue);
                                            updateCmd.Parameters.AddWithValue("@recordId", recordId);
                                            updateCmd.ExecuteNonQuery();

                                            // Sync back to audit
                                            parameterValues[att.ColName] = finalValue;
                                        }
                                    }


                                    insertedIds[$"{tableName}_{rowIndex}"] = recordId;

                                    var rowSnapshot = new Dictionary<string, object>(parameterValues)
                                    {
                                        ["RecordID"] = recordId,
                                        ["StepDescription"] = stepDescription,
                                        ["interactionType"] = recordId > 0 ? "Update" : "Insert",
                                        ["CreatedUserID"] = userId,
                                        ["CreatedDate"] = DateTime.Now
                                    };

                                    string json = JsonConvert.SerializeObject(rowSnapshot);

                                    using var auditCmd = new SqlCommand(@"
                                INSERT INTO foProcessEventsDetail 
                                (ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active) 
                                VALUES (@ProcessEventID, @ProcessInstanceID, @StepID, @TableName, @RecordID, @DataSetUpdate, GETDATE(), @CreatedUserID, 1)", conn, transaction);
                                    auditCmd.Parameters.AddWithValue("@ProcessEventID", processEventId);
                                    auditCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                    auditCmd.Parameters.AddWithValue("@StepID", stepId);
                                    auditCmd.Parameters.AddWithValue("@TableName", tableName);
                                    auditCmd.Parameters.AddWithValue("@RecordID", recordId);
                                    auditCmd.Parameters.AddWithValue("@DataSetUpdate", json);
                                    auditCmd.Parameters.AddWithValue("@CreatedUserID", userId);
                                    auditCmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    rowErrors.Add($"Error saving '{tableName}' row {rowIndex}: {ex.Message}");
                                }
                            }
                        }

                        if (rowErrors.Any())
                        {
                            transaction.Rollback();
                            TempData["RowErrors"] = rowErrors;
                            return RedirectToAction("ExecuteStep", new { stepId, processInstanceId, userId });
                        }

                        int previousEventID;
                        using (var getEventCmd = new SqlCommand(@"
                        SELECT PreviousEventID FROM foProcessEvents d 
                        WHERE ID = @EventID 
                        ORDER BY d.ID DESC", conn, transaction))
                        {
                            getEventCmd.Parameters.AddWithValue("@EventID", EventID);
                            previousEventID = Convert.ToInt32(getEventCmd.ExecuteScalar());
                        }

                        if (previousEventID < 0)
                        {
                            var approvers = new List<int>();
                            var assignees = new List<string>();

                            // 🔍 Step 1: Get distinct UserIDs
                            var getApproversCmd = new SqlCommand(@"
        SELECT DISTINCT ae.UserID
        FROM foApprovalEvents ae
        WHERE ae.PreviousEventID IN (
            SELECT ae2.PreviousEventID
            FROM foApprovalEvents ae2
            WHERE ae2.ID IN (
                SELECT -pe.PreviousEventID
                FROM foProcessEvents pe
                WHERE pe.ID = @EventID AND pe.Active = 1
            )
            AND ae2.StepID = 0
        )", conn, transaction);
                            getApproversCmd.Parameters.AddWithValue("@EventID", EventID);

                            using (var reader = getApproversCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["UserID"] != DBNull.Value)
                                        approvers.Add(Convert.ToInt32(reader["UserID"]));
                                }
                            }

                            // 🔁 Step 2: Reinsert approvers and build assignee list
                            foreach (var approverId in approvers)
                            {
                                var insertCmd = new SqlCommand(@"
                                INSERT INTO foApprovalEvents 
                                (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, Active)
                                VALUES (@ProcessInstanceID, 0, @PreviousEventID, @UserID, GETDATE(), 1)", conn, transaction);

                                insertCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                insertCmd.Parameters.AddWithValue("@PreviousEventID", -EventID);
                                insertCmd.Parameters.AddWithValue("@UserID", approverId);
                                insertCmd.ExecuteNonQuery();

                                // 🔍 Step 3: Get user name
                                var userCmd = new SqlCommand("SELECT FirstName, LastName FROM foUsers WHERE ID = @UserID", conn, transaction);
                                userCmd.Parameters.AddWithValue("@UserID", approverId);
                                using var userReader = userCmd.ExecuteReader();
                                if (userReader.Read())
                                {
                                    var name = $"👤 {userReader["FirstName"]} {userReader["LastName"]}";
                                    assignees.Add(name);
                                }
                            }

                            // ✅ Mark the process event as done
                            var markDoneCmd = new SqlCommand(@"
                            UPDATE foProcessEvents
                            SET DateCompleted = GETDATE()
                            WHERE ID = @EventID", conn, transaction);
                            markDoneCmd.Parameters.AddWithValue("@EventID", EventID);
                            markDoneCmd.ExecuteNonQuery();

                            //   var transitionResult_rework = HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId, action, SendForApproval, SelectedApproverIds);

                            //   transaction.Commit();

                            // ✅ Compose final message
                            ViewBag.action = "Step resubmitted successfully";
                            string message = "Your step has been resubmitted";

                            if (assignees.Any())
                            {
                                message += "<br>Reassigned to:<br> - " + string.Join("<br>- ", assignees);
                            }

                            //return RedirectToAction("StepCompleted", "StepCompleted", new
                            //{
                            //    message,
                            //    userId,
                            //    actionheader = ViewBag.action
                            //});
                        }


                        // ✅ Always save data first above
                        // ✅ Now handle what happens after save

                        //var transitionResult = HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId, action, form["cancelReason"]);
                        var transitionResult = HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId, action, SendForApproval, SelectedApproverIds, form["cancelReason"]);

                        // ✅ Fetch next step assignment BEFORE committing the transaction
                        string nextAssignmentMessage = GetNextStepAssignment(conn, transaction, processEventId, action);

                        // ✅ Now safe to commit
                        transaction.Commit();

                        // Store message in TempData so it can survive redirect
                        TempData["SuccessMessage"] = nextAssignmentMessage;
                        TempData["UserId"] = userId;

                        if (action == "SaveLater")
                        {
                            ViewBag.action = "Step Saved Successully";
                        }
                        else if (action == "CancelProcess")
                        {
                            ViewBag.action = "Process Cancelled Successully";
                        }
                        else if (action == "SaveContinue")
                        {
                            ViewBag.action = "Step Submitted Successully";
                        }

                        return RedirectToAction("StepCompleted", "StepCompleted", new { message = nextAssignmentMessage, userId, actionheader = ViewBag.action });

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["RowErrors"] = new List<string> { ex.Message };
                        return RedirectToAction("Error", new { stepId, processInstanceId, userId });
                    }
                }
            }
        }



        private StepTransitionResult HandleNextStep(SqlConnection conn, SqlTransaction transaction, int currentStepId, int currentEventId, int? processInstanceId, int userId, string action, bool SendForApproval = false, List<int> SelectedApproverIds = null, string cancelComment = null)
        {
            var result = new StepTransitionResult();

            // 🔥 1. Handle cancellation immediately
            if (action == "CancelProcess")
            {
                using (var cancelCmd = new SqlCommand(@"
            UPDATE foProcessEvents
            SET Active = 0, Cancelled = 1, DateCompleted = GETDATE()
            WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction))
                {
                    cancelCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    cancelCmd.ExecuteNonQuery();
                }

                using (var insertCancelCmd = new SqlCommand(@"
            INSERT INTO foProcessCancellations (ProcessInstanceID, CancelledUserID, CancellationReason)
            VALUES (@ProcessInstanceID, @CancelledUserID, @CancellationReason)", conn, transaction))
                {
                    insertCancelCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertCancelCmd.Parameters.AddWithValue("@CancelledUserID", userId);
                    insertCancelCmd.Parameters.AddWithValue("@CancellationReason", cancelComment ?? "Cancelled by user");
                    insertCancelCmd.ExecuteNonQuery();
                }

                ArchiveProcess(conn, transaction, processInstanceId);
                result.Cancelled = true;
                return result;
            }

            // 🔥 2. Mark current step as complete
            using (var markDoneCmd = new SqlCommand(@"
        UPDATE foProcessEvents
        SET DateCompleted = GETDATE()
        WHERE ID = @EventID", conn, transaction))
            {
                markDoneCmd.Parameters.AddWithValue("@EventID", currentEventId);
                markDoneCmd.ExecuteNonQuery();
            }

            // 🔁 3. Handle SaveLater immediately
            if (action == "SaveLater")
            {
                using (var insertSelfCmd = new SqlCommand(@"
            INSERT INTO foProcessEvents
            (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
            VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, NULL, @UserID, GETDATE(), 1)", conn, transaction))
                {
                    insertSelfCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertSelfCmd.Parameters.AddWithValue("@StepID", currentStepId);
                    insertSelfCmd.Parameters.AddWithValue("@PreviousEventID", currentEventId);
                    insertSelfCmd.Parameters.AddWithValue("@UserID", userId);
                    insertSelfCmd.ExecuteNonQuery();
                }

                result.NextStepExists = true;
                return result;
            }

            // 🔄 4. Try get next step
            long? nextStepId = null;
            object nextGroupId = DBNull.Value, nextUserId = DBNull.Value;

            using (var getNextCmd = new SqlCommand(@"
        SELECT TOP 1 ID, GroupID, UserID
        FROM foProcessSteps
        WHERE ProcessID = (SELECT ProcessID FROM foProcessSteps WHERE ID = @CurrentStepID)
          AND StepNo > (SELECT StepNo FROM foProcessSteps WHERE ID = @CurrentStepID)
        ORDER BY StepNo", conn, transaction))
            {
                getNextCmd.Parameters.AddWithValue("@CurrentStepID", currentStepId);
                using (var reader = getNextCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nextStepId = reader.GetInt64(0);
                        nextGroupId = reader["GroupID"] ?? DBNull.Value;
                        nextUserId = reader["UserID"] ?? DBNull.Value;
                    }
                }
            }

            if (nextStepId.HasValue)
            {
                using (var insertNextCmd = new SqlCommand(@"
            INSERT INTO foProcessEvents
            (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
            VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE(), 1)", conn, transaction))
                {
                    insertNextCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertNextCmd.Parameters.AddWithValue("@StepID", (int)nextStepId.Value);
                    insertNextCmd.Parameters.AddWithValue("@PreviousEventID", currentEventId);
                    insertNextCmd.Parameters.AddWithValue("@GroupID", nextGroupId);
                    insertNextCmd.Parameters.AddWithValue("@UserID", nextUserId);
                    insertNextCmd.ExecuteNonQuery();
                }

                result.NextStepExists = true;
            }
            else
            {
                // 🔍 5. No next step — check ad-hoc or predefined approvals
                if (SendForApproval && SelectedApproverIds?.Any() == true)
                {
                    foreach (var approverId in SelectedApproverIds)
                    {
                        using (var insertApprovalCmd = new SqlCommand(@"
                    INSERT INTO foApprovalEvents
                    (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
                    VALUES (@ProcessInstanceID, 0, @PreviousEventID, NULL, @UserID, GETDATE(), 1)", conn, transaction))
                        {
                            insertApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            insertApprovalCmd.Parameters.AddWithValue("@PreviousEventID", -currentEventId);
                            insertApprovalCmd.Parameters.AddWithValue("@UserID", approverId);
                            insertApprovalCmd.ExecuteNonQuery();
                        }
                    }

                    result.ApprovalStarted = true;
                }
                else
                {
                    int processId;
                    using (var processIdCmd = new SqlCommand("SELECT ProcessID FROM foProcessSteps WHERE ID = @CurrentStepID", conn, transaction))
                    {
                        processIdCmd.Parameters.AddWithValue("@CurrentStepID", currentStepId);
                        processId = Convert.ToInt32(processIdCmd.ExecuteScalar());
                    }

                    int? approvalStepId = null;
                    object groupId = DBNull.Value, userIdApproval = DBNull.Value;

                    using (var approvalStepsCmd = new SqlCommand(@"
                SELECT TOP 1 ID, GroupID, UserID
                FROM foApprovalSteps
                WHERE ProcessID = @ProcessID AND Active = 1
                ORDER BY StepNo", conn, transaction))
                    {
                        approvalStepsCmd.Parameters.AddWithValue("@ProcessID", processId);
                        using (var reader = approvalStepsCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                approvalStepId = Convert.ToInt32(reader["ID"]);
                                groupId = reader["GroupID"] ?? DBNull.Value;
                                userIdApproval = reader["UserID"] ?? DBNull.Value;
                            }
                        }
                    }

                    if (approvalStepId.HasValue)
                    {
                        using (var insertApprovalEventCmd = new SqlCommand(@"
                    INSERT INTO foApprovalEvents
                    (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
                    VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE(), 1)", conn, transaction))
                        {
                            insertApprovalEventCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            insertApprovalEventCmd.Parameters.AddWithValue("@StepID", approvalStepId.Value);
                            insertApprovalEventCmd.Parameters.AddWithValue("@PreviousEventID", -currentEventId);
                            insertApprovalEventCmd.Parameters.AddWithValue("@GroupID", groupId);
                            insertApprovalEventCmd.Parameters.AddWithValue("@UserID", userIdApproval);
                            insertApprovalEventCmd.ExecuteNonQuery();
                        }

                        result.ApprovalStarted = true;
                    }
                    else
                    {
                        ArchiveProcess(conn, transaction, processInstanceId);
                        result.ApprovalStarted = false;
                    }
                }
            }

            return result;
        }

        private string GetForeignKeyDescription(string tableName, string idValue)
        {
            if (string.IsNullOrWhiteSpace(idValue))
                return "";

            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"
                SELECT TOP 1 
                    ISNULL(CONCAT(FirstName, ' ', LastName), '') AS FullName, 
                    ISNULL(Name, '') AS NameField, 
                    ISNULL(Description, '') AS DescriptionField 
                FROM " + tableName + @" 
                WHERE ID = @ID", conn);

                    cmd.Parameters.AddWithValue("@ID", idValue);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var fullName = reader["FullName"]?.ToString();
                            var name = reader["NameField"]?.ToString();
                            var description = reader["DescriptionField"]?.ToString();

                            // Prioritize: FullName > NameField > DescriptionField > Fallback
                            if (!string.IsNullOrWhiteSpace(fullName)) return fullName;
                            if (!string.IsNullOrWhiteSpace(name)) return name;
                            if (!string.IsNullOrWhiteSpace(description)) return description;

                            return $"Record {idValue}";
                        }
                    }
                }
            }
            catch
            {
                // Fallback in case of error
                return $"Record {idValue}";
            }

            return $"Record {idValue}";
        }



        private List<SelectListItem> GetForeignKeyOptions(string tableName)
        {
            var options = new List<SelectListItem>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                var fkColumns = GetForeignKeyColumns(tableName).ToList();
                var fkLookup = fkColumns.ToDictionary(fk => fk.ColumnName, fk => fk.TableName, StringComparer.OrdinalIgnoreCase);

                // Get available columns for display
                var columnsQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                var displayColumns = new List<string>();
                using (var cmd = new SqlCommand(columnsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = reader["COLUMN_NAME"].ToString();
                            if (!col.Equals("ID", StringComparison.OrdinalIgnoreCase) && !ignoredSet.Contains(col))
                            {
                                displayColumns.Add(col);
                            }
                        }
                    }
                }

                if (displayColumns.Count == 0)
                    displayColumns.Add("ID"); // Fallback

                // Build SELECT query
                var selectColumns = "ID, " + string.Join(", ", displayColumns);
                var query = $"SELECT {selectColumns} FROM {tableName} WHERE Active = 1";

                using (var cmd = new SqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var displayParts = new List<string>();
                        foreach (var col in displayColumns)
                        {
                            var val = reader[col]?.ToString();
                            if (!string.IsNullOrEmpty(val) && fkLookup.ContainsKey(col))
                            {
                                // Foreign key column: Resolve FK description
                                var fkTable = fkLookup[col];
                                var fkOptions = GetForeignKeyOptions(fkTable);
                                var match = fkOptions.FirstOrDefault(x => x.Value == val);
                                if (match != null)
                                    val = match.Text;
                            }

                            displayParts.Add(val);
                        }

                        options.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = string.Join(" | ", displayParts)
                        });
                    }
                }
            }

            return options;
        }


        private List<Dictionary<string, object>> GetForeignKeySearchData(string tableName)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // 🔍 Check if it's a transaction table
                string prefixQuery = "SELECT Description FROM foTablePrefixes WHERE @TableName LIKE Prefix + '%' AND Active = 1";
                string tableType = null;
                using (var prefixCmd = new SqlCommand(prefixQuery, connection))
                {
                    prefixCmd.Parameters.AddWithValue("@TableName", tableName);
                    tableType = prefixCmd.ExecuteScalar()?.ToString();
                }

                if (tableType == "Maintenance")
                {
                    throw new InvalidOperationException("This table is a maintenance table. Use GetForeignKeyOptions for dropdowns.");
                }

                var data = new List<Dictionary<string, object>>();
                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                var columnsQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                var displayColumns = new List<string>();
                using (var cmd = new SqlCommand(columnsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = reader["COLUMN_NAME"].ToString();
                            if (!col.Equals("ID", StringComparison.OrdinalIgnoreCase) &&
                                !col.EndsWith("ID", StringComparison.OrdinalIgnoreCase) &&
                                !ignoredSet.Contains(col))
                            {
                                displayColumns.Add(col);
                            }
                        }
                    }
                }

                if (displayColumns.Count == 0)
                    displayColumns.Add("ID");

                string selectColumns = "ID, " + string.Join(", ", displayColumns);
                string query = $"SELECT {selectColumns} FROM {tableName} WHERE Active = 1";

                using (var cmd = new SqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>
                        {
                            ["ID"] = reader["ID"].ToString()
                        };

                        foreach (var col in displayColumns)
                        {
                            row[col] = reader[col]?.ToString();
                        }

                        data.Add(row);
                    }
                }

                return data;
            }
        }



        private List<string> GetAllColumnsForTable(string tableName, SqlConnection conn, string columnQuery = "*")
        {
            var allColumns = new List<string>();

            var query = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allColumns.Add(reader.GetString(0));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(columnQuery) && columnQuery.Trim() != "*")
            {
                var requested = columnQuery
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => c.Trim())
                    .ToList();

                return allColumns.Where(c => requested.Contains(c, StringComparer.OrdinalIgnoreCase)).ToList();
            }

            return allColumns;
        }

        private List<string> GetIgnoredColumns(SqlConnection conn)
        {
            var ignoredColumns = new List<string>();
            string query = "SELECT ColumnName FROM foTableColumnsToIgnore";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ignoredColumns.Add(reader["ColumnName"].ToString());
                    }
                }
            }

            return ignoredColumns;
        }


        private List<ForeignKeyInfo> GetForeignKeyColumns(string tablename)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            string query = @"
        SELECT 
            c.name AS ColumnName,
            ref_tab.name AS ReferencedTableName,
            CASE WHEN RIGHT(c.name, 2) = 'ID' THEN LEFT(c.name, LEN(c.name) - 2) ELSE c.name END AS ColumnDescription
        FROM 
            sys.foreign_key_columns AS fkc
        INNER JOIN 
            sys.columns AS c ON fkc.parent_object_id = c.object_id 
            AND fkc.parent_column_id = c.column_id
        INNER JOIN 
            sys.tables AS parent_tab ON parent_tab.object_id = fkc.parent_object_id
        INNER JOIN 
            sys.tables AS ref_tab ON ref_tab.object_id = fkc.referenced_object_id
        WHERE 
            parent_tab.name = @TableName AND c.name LIKE '%ID'";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreignKeys.Add(new ForeignKeyInfo
                            {
                                ColumnName = reader.GetString(0),
                                TableName = reader.GetString(1),
                                ColumnDescription = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return foreignKeys;
        }
        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            return _dbHelper.ExecuteQuery<foTablePrefix>(query);
        }

        private string CleanTableName(string tableName, List<foTablePrefix> tablePrefixes)
        {
            foreach (var prefix in tablePrefixes)
            {
                if (tableName.StartsWith(prefix.Prefix))
                {
                    return tableName.Replace(prefix.Prefix, "").Replace("_", " ");
                }
            }
            return tableName.Replace("_", " ");
        }
        public class StepTransitionResult
        {
            public bool NextStepExists { get; set; }
            public bool ApprovalStarted { get; set; }
            public bool Cancelled { get; set; }
        }
        private void ArchiveProcess(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            // 🟰 Copy Process Events into Archive
            var archiveEventsCmd = new SqlCommand(@"
        INSERT INTO foProcessEventsArchive (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active, Cancelled)
        SELECT ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active, Cancelled
        FROM foProcessEvents
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveEventsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveEventsCmd.ExecuteNonQuery();

            // 🟰 Copy Process Event Details into Archive
            var archiveDetailsCmd = new SqlCommand(@"
        INSERT INTO foProcessEventsDetailArchive (ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
        SELECT ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active
        FROM foProcessEventsDetail
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveDetailsCmd.ExecuteNonQuery();

            // 🟰 Copy Approval Events into Archive
            var archiveApprovalCmd = new SqlCommand(@"
        INSERT INTO foApprovalEventsArchive (ProcessInstanceID, StepID,PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active)
        SELECT ProcessInstanceID, StepID,PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active
        FROM foApprovalEvents
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveApprovalCmd.ExecuteNonQuery();

            // 🟰 Copy Approval Event Details into Archive
            var archiveApprovalDetailsCmd = new SqlCommand(@"
        INSERT INTO foApprovalEventsDetailArchive (ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
        SELECT ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active
        FROM foApprovalEventsDetail
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveApprovalDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveApprovalDetailsCmd.ExecuteNonQuery();

            // 🔥 Delete or deactivate process events
            var deleteProcEventsCmd = new SqlCommand("DELETE FROM foProcessEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteProcEventsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteProcEventsCmd.ExecuteNonQuery();

            var deleteProcDetailsCmd = new SqlCommand("DELETE FROM foProcessEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteProcDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteProcDetailsCmd.ExecuteNonQuery();

            // 🔥 Delete or deactivate approval events
            var deleteApprovalCmd = new SqlCommand("DELETE FROM foApprovalEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteApprovalCmd.ExecuteNonQuery();

            var deleteApprovalDetailsCmd = new SqlCommand("DELETE FROM foApprovalEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteApprovalDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteApprovalDetailsCmd.ExecuteNonQuery();
        }



        private void PopulateProcessHistory(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            var ignoredColumns = GetIgnoredColumns(conn);

            if (!processInstanceId.HasValue) return;

            var history = new List<ProcessEventAuditViewModel>();

            var cmd = new SqlCommand(@"
        SELECT a.id AS DetailID, ProcessEventID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo, c.FirstName + ' '+ c.LastName AS FullName
    FROM foProcessEventsDetail a LEFT OUTER JOIN foProcessSteps b on a.StepID = b.ID
    LEFT JOIN foUsers c on c.ID = a.CreatedUserID
    WHERE ProcessInstanceID = @ProcessInstanceID
    ORDER BY CreatedDate", conn, transaction);

            cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    // Filter ignored columns from the dictionary
                    var data = rawData
                        .Where(kvp => !ignoredColumns.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    history.Add(new ProcessEventAuditViewModel
                    {
                        DetailID = Convert.ToInt32(reader["DetailID"]),
                        ProcessEventID = Convert.ToInt32(reader["ProcessEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        FullName = reader["FullName"].ToString(),
                        Data = data
                    });
                }
            }

            // 🔹 APPROVAL EVENTS
            var approvalCmd = new SqlCommand(@"
        SELECT  a.id AS DetailID, ApprovalEventID, StepID, 'foApproval' AS TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, ISNULL ( StepNo,'1.00') AS StepNo, c.FirstName + ' '+ c.LastName AS FullName
FROM foApprovalEventsDetail a LEFT OUTER JOIN foApprovalSteps b on a.StepID = b.ID
LEFT JOIN foUsers c on c.ID = a.CreatedUserID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            approvalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            var approvalHistory = new List<ProcessEventAuditViewModel>();
            using (var reader = approvalCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    // Filter ignored columns from the dictionary
                    var data = rawData
                        .Where(kvp => !ignoredColumns.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    data["StepType"] = "Approval";

                    approvalHistory.Add(new ProcessEventAuditViewModel
                    {
                        DetailID = Convert.ToInt32(reader["DetailID"]),
                        ProcessEventID = Convert.ToInt32(reader["ApprovalEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        FullName = reader["FullName"].ToString(),

                        Data = data
                    });
                }
            }

            // Now, enrich data: resolve FKs and attachments
            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            var usedTables = history.Select(h => h.TableName).Distinct();

            foreach (var table in usedTables)
            {
                foreignKeys[table] = GetForeignKeyColumns(table)
                    .Where(fk => fk.TableName.Contains("_md_"))
                    .ToList();

                foreach (var fk in foreignKeys[table])
                {
                    if (!foreignKeyOptions.ContainsKey(fk.TableName))
                    {
                        foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                    }
                }
            }

            foreach (var record in history)
            {
                if (foreignKeys.TryGetValue(record.TableName, out var fks))
                {
                    foreach (var fk in fks)
                    {
                        if (record.Data.TryGetValue(fk.ColumnName, out var fkVal) &&
                            fkVal != null &&
                            int.TryParse(fkVal.ToString(), out var fkId))
                        {
                            var label = foreignKeyOptions[fk.TableName]
                                .FirstOrDefault(o => o.Value == fkId.ToString())?.Text;

                            if (!string.IsNullOrWhiteSpace(label))
                            {
                                record.Data[fk.ColumnName] = $"{label} (ID: {fkId})";
                            }
                        }
                    }
                }

                // Resolve attachment fields
                foreach (var key in record.Data.Keys.Where(k => k.StartsWith("attachment_")).ToList())
                {
                    var val = record.Data[key]?.ToString();
                    if (!string.IsNullOrEmpty(val) && val.Contains(';'))
                    {
                        var parts = val.Split(';');
                        var desc = parts[0];
                        var path = parts.Length > 1 ? parts[1] : "";
                        if (!string.IsNullOrEmpty(path))
                        {
                            var filename = System.IO.Path.GetFileName(path);
                            record.Data[key] = $"<a href='/Attachments/{path}' target='_blank'>{desc ?? filename}</a>";
                        }
                        else
                        {
                            record.Data[key] = desc;
                        }
                    }
                }
            }

            ViewBag.ProcessHistory = history;

        }

        private List<foProcessDetail> GetProcessTables(int stepId, SqlConnection conn, SqlTransaction tx)
        {
            var result = new List<foProcessDetail>();
            var cmd = new SqlCommand("SELECT * FROM foProcessDetail WHERE StepID = @StepID", conn, tx);
            cmd.Parameters.AddWithValue("@StepID", stepId);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new foProcessDetail
                {
                    TableName = reader["TableName"].ToString(),
                    FormType = reader["FormType"].ToString()
                });
            }
            reader.Close();
            return result;
        }

        private List<(string ColumnName, string DataType)> GetColumnDefinitions(string tableName, SqlConnection conn, SqlTransaction tx)
        {
            var result = new List<(string, string)>();
            var cmd = new SqlCommand(@"
            SELECT COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @TableName
              AND COLUMN_NAME NOT IN ('ID', 'Active')", conn, tx);
            cmd.Parameters.AddWithValue("@TableName", tableName);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add((reader["COLUMN_NAME"].ToString(), reader["DATA_TYPE"].ToString()));
            }
            reader.Close();
            return result;
        }

        [HttpPost]
        public IActionResult AddTableRowPartial([FromBody] AddRowRequestModel model)
        {


            using var conn = GetConnection();
            conn.Open();

            var ignoredColumns = GetIgnoredColumns(conn);

            // Query to get specific table for step
            var query = @"SELECT TOP 1 TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
                  FROM foProcessDetail 
                  WHERE StepID = @StepID AND TableName = @TableName AND Active = 1";

            foProcessDetail table;
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StepID", model.StepId);
                cmd.Parameters.AddWithValue("@TableName", model.TableName);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return BadRequest("Table not found in step.");

                table = new foProcessDetail
                {
                    TableName = reader["TableName"].ToString(),
                    ColumnQuery = reader["ColumnQuery"].ToString(),
                    FormType = reader["FormType"].ToString(),
                    ColumnCount = reader["ColumnCount"] != DBNull.Value ? Convert.ToInt32(reader["ColumnCount"]) : (int?)null,
                    Parent = Convert.ToBoolean(reader["Parent"]),
                    FKColumn = reader["FKColumn"].ToString(),
                    TableDescription = reader["TableDescription"].ToString()
                };
            }

            if (!string.IsNullOrEmpty(table.FKColumn))
                ignoredColumns.Add(table.FKColumn);

            List<string> columns;
            if (table.ColumnQuery.Trim() == "*")
            {
                columns = GetAllColumnsForTable(table.TableName, conn)
                    .Where(col => !ignoredColumns.Contains(col) && !col.Equals("ID", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                columns = table.ColumnQuery.Split(',')
                    .Select(c => c.Trim())
                    .Where(col => !ignoredColumns.Contains(col) && !col.Equals("ID", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var columnTypes = new Dictionary<string, string>();
            var columnLengths = new Dictionary<string, int>();

            using (var cmd = new SqlCommand(@"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
                                      FROM INFORMATION_SCHEMA.COLUMNS
                                      WHERE TABLE_NAME = @TableName", conn))
            {
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var col = reader["COLUMN_NAME"].ToString();
                    columnTypes[col] = reader["DATA_TYPE"].ToString();
                    columnLengths[col] = reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value
                        ? Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : 255;
                }
            }
            var foreignKeys = GetForeignKeyColumns(table.TableName)
                .Where(fk => fk.TableName.Contains("_md_"))
                .ToList();

            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                if (!foreignKeyOptions.ContainsKey(fk.TableName))
                {
                    foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                }
            }

            var vm = new DynamicTableViewModel
            {
                TableName = table.TableName,
                Columns = columns,
                RowCount =  1,
                StartIndex = model.CurrentRowCount,
                ColumnTypes = columnTypes,
                ColumnLengths = columnLengths,
                ForeignKeys = foreignKeys,
                ForeignKeyOptions = foreignKeyOptions
            };

            return PartialView("_TableRows", vm);
        }




       

        [HttpPost]
        public IActionResult ClaimStep(int EventID, int userId, int stepId, int processInstanceId, int processId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
        UPDATE foProcessEvents
        SET UserID = @UserID, GroupID = NULL
        WHERE ID = @id AND UserID IS NULL", conn);

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@id", EventID);
            cmd.ExecuteNonQuery();

            // Redirect straight to the step form after claiming
            return RedirectToAction("PendingStep", new
            {
                processId,
                stepId,
                processInstanceId,
                userId
            });
        }

        private List<SelectListItem> GetApproverSelectList()
        {
            var list = new List<SelectListItem>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers WHERE Active = 1 ORDER BY 2", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["FullName"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        private string GetNextStepAssignment(SqlConnection conn, SqlTransaction transaction, int currentEventId, string action, string overrideMessage = null)
        {
            var assignees = new List<string>();
            string stepDescription = null;
            DateTime? dateCompleted = null;

            // 🔍 Fetch timestamp from current event
            using (var tsCmd = new SqlCommand(@"SELECT DateCompleted FROM foProcessEvents WHERE ID = @ID", conn, transaction))
            {
                tsCmd.Parameters.AddWithValue("@ID", currentEventId);
                var result = tsCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    dateCompleted = Convert.ToDateTime(result);
            }

            // 1️⃣ Process Steps
            using (var processCmd = new SqlCommand(@"
        SELECT pe.UserID, u.FirstName, u.LastName, 
               pe.GroupID, g.Description AS GroupName, 
               s.StepDescription,
               pe.DateCompleted
        FROM foProcessEvents pe
        LEFT JOIN foUsers u ON pe.UserID = u.ID
        LEFT JOIN foGroups g ON pe.GroupID = g.ID
        LEFT JOIN foProcessSteps s ON s.ID = pe.StepID
        WHERE pe.PreviousEventID = @EventID", conn, transaction))
            {
                processCmd.Parameters.AddWithValue("@EventID", currentEventId);
                using var reader = processCmd.ExecuteReader();
                while (reader.Read())
                {
                    stepDescription ??= reader["StepDescription"]?.ToString() ?? "(Unnamed Step)";
                    dateCompleted ??= reader["DateCompleted"] != DBNull.Value ? Convert.ToDateTime(reader["DateCompleted"]) : (DateTime?)null;

                    if (reader["UserID"] != DBNull.Value)
                        assignees.Add($"👤 {reader["FirstName"]} {reader["LastName"]}");
                    else if (reader["GroupID"] != DBNull.Value)
                        assignees.Add($"🧑‍🤝‍🧑 {reader["GroupName"]}");
                }
            }

            // 2️⃣ Approval Steps
            using (var approvalCmd = new SqlCommand(@"
        SELECT ae.UserID, u.FirstName, u.LastName,
               ae.GroupID, g.Description AS GroupName,
               s.StepDescription,
               ae.DateCompleted
        FROM foApprovalEvents ae
        LEFT JOIN foUsers u ON ae.UserID = u.ID
        LEFT JOIN foGroups g ON ae.GroupID = g.ID
        LEFT JOIN foApprovalSteps s ON s.ID = ae.StepID
        WHERE ae.PreviousEventID = @EventID", conn, transaction))
            {
                approvalCmd.Parameters.AddWithValue("@EventID", -currentEventId);
                using var reader = approvalCmd.ExecuteReader();
                while (reader.Read())
                {
                    stepDescription ??= reader["StepDescription"]?.ToString() ?? "(Unnamed Approval Step)";
                    dateCompleted ??= reader["DateCompleted"] != DBNull.Value ? Convert.ToDateTime(reader["DateCompleted"]) : (DateTime?)null;

                    if (reader["UserID"] != DBNull.Value)
                        assignees.Add($"👤 {reader["FirstName"]} {reader["LastName"]}");
                    else if (reader["GroupID"] != DBNull.Value)
                        assignees.Add($"🧑‍🤝‍🧑 {reader["GroupName"]}");
                }
            }

            // ✅ Final Footer with Timestamp + Event ID
            string timestamp = dateCompleted.HasValue
                ? $"<div><small class='text-muted'>Submitted on {dateCompleted.Value:yyyy-MM-dd HH:mm:ss}</small></div>"
                : "";
            string footer = $@"<hr>{timestamp}
                       <div><small class='text-muted'>ID: {currentEventId}</small></div>";

            string message = "";
            if (action == "SaveLater")
            {
                message = "Your step has been saved";
            }
            else if (action == "CancelProcess")
            {
                message = "The process has been cancelled";
            }
            else if (action == "SaveContinue")
            {
                message = "Your step has been submitted";
            }

            // ✅ Final Message
            if (assignees.Any())
            {
                string assigneeList = string.Join("<br>- ", assignees);
                return $@"✅ {message}.<br>
                  The next step is <strong>{stepDescription}</strong><br>
                  Assigned to:<br> {assigneeList}
                  {footer}";
            }
            else
            {
                return $@"✅ {message}.<br>
                  Process has been completed.
                  {footer}";
            }
        }

        public IActionResult GetReferenceData(string tableName)
        {
            try
            {
                var data = GetForeignKeyOptions(tableName);
                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error fetching reference data: " + ex.Message);
            }
        }


        [HttpGet]
        public IActionResult GetSearchOptions(string tableName, string columnName)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    var ignoredColumns = GetIgnoredColumns(conn);
                    var fkColumns = GetForeignKeyColumns(tableName).ToList();
                    var fkOptions = new Dictionary<string, List<SelectListItem>>();

                    foreach (var fk in fkColumns)
                    {
                        if (!fkOptions.ContainsKey(fk.TableName))
                        {
                            fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                        }
                    }

                    var cmd = new SqlCommand($"SELECT * FROM {tableName} WHERE Active = 1", conn);
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        row["ID"] = reader["ID"].ToString();

                        var displayParts = new List<string>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);

                            if (!ignoredColumns.Contains(name) && !name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                            {
                                var value = reader[name]?.ToString();
                                var fkMatch = fkColumns.FirstOrDefault(x => x.ColumnName == name);

                                if (fkMatch != null && !string.IsNullOrEmpty(value))
                                {
                                    var displayValue = fkOptions[fkMatch.TableName]
                                        .FirstOrDefault(x => x.Value == value)?.Text ?? value;
                                    displayParts.Add(displayValue);
                                }
                                else
                                {
                                    displayParts.Add(value);
                                }
                            }
                        }

                        row["Display"] = string.Join(" | ", displayParts);
                        result.Add(row);
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }






    }

}
