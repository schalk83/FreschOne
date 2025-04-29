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

                var query = @"SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
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
                                TableDescription = reader["TableDescription"].ToString()
                            });
                        }
                    }
                }

                foreach (var table in tables)
                {
                    if (!string.IsNullOrEmpty(table.FKColumn))
                        ignoredColumns.Add(table.FKColumn);

                    foreignKeys[table.TableName] = GetForeignKeyColumns(table.TableName)
                        .Where(fk => fk.TableName.Contains("_md_"))
                        .ToList();

                    foreach (var fk in foreignKeys[table.TableName])
                    {
                        if (!foreignKeyOptions.ContainsKey(fk.TableName))
                        {
                            foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                        }
                    }

                    List<string> columns;
                    if (table.ColumnQuery.Trim() == "*")
                    {
                        columns = GetAllColumnsForTable(table.TableName, conn)
                            .Where(col => !ignoredColumns.Contains(col))
                            .ToList();
                    }
                    else
                    {
                        columns = table.ColumnQuery.Split(',')
                            .Select(col => col.Trim())
                            .Where(col => !ignoredColumns.Contains(col))
                            .ToList();
                    }

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

                    var row = new Dictionary<string, object>();
                    foreach (var col in columns)
                    {
                        row[col] = null;
                    }

                    tableData[table.TableName] = new List<Dictionary<string, object>> { row };

                    // 🔥 Force StartIndex = 0 when initially loading CreateStep
                    var model = new DynamicTableViewModel
                    {
                        TableName = table.TableName,
                        Columns = columns,
                        RowCount = 1, // Only 1 blank row initially
                        StartIndex = 0, // 🟢 Here: Start at 0
                        ColumnTypes = columnTypes.ContainsKey(table.TableName) ? columnTypes[table.TableName] : new Dictionary<string, string>(),
                        ColumnLengths = columnLengths.ContainsKey(table.TableName) ? columnLengths[table.TableName] : new Dictionary<string, int>(),
                        ForeignKeys = foreignKeys.ContainsKey(table.TableName) ? foreignKeys[table.TableName] : new List<ForeignKeyInfo>(),
                        ForeignKeyOptions = foreignKeyOptions
                    };

                    ViewBag.InitialModels ??= new List<DynamicTableViewModel>();
                    (ViewBag.InitialModels as List<DynamicTableViewModel>).Add(model);
                }
            }

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
            SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
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
                                TableDescription = reader["TableDescription"].ToString()
                            });
                        }
                    }
                }

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

                    foreignKeys[table.TableName] = GetForeignKeyColumns(table.TableName).Where(fk => fk.TableName.Contains("_md_")).ToList();
                    foreach (var fk in foreignKeys[table.TableName])
                    {
                        if (!foreignKeyOptions.ContainsKey(fk.TableName))
                            foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                    }

                    var rows = new List<Dictionary<string, object>>();

                    if (table.FormType == "F")
                    {
                        // 🟢 Freeform: Only latest record
                        var latestCmd = new SqlCommand(@"
                    SELECT RecordID, DataSetUpdate 
                    FROM foProcessEventsDetail 
                    WHERE ProcessInstanceID = @InstanceID 
                      AND TableName = @TableName 
                      AND Active = 1
                      AND ID = (
                          SELECT MAX(ID) 
                          FROM foProcessEventsDetail 
                          WHERE ProcessInstanceID = @InstanceID 
                            AND TableName = @TableName 
                            AND Active = 1
                      )", conn);

                        latestCmd.Parameters.AddWithValue("@InstanceID", processInstanceId);
                        latestCmd.Parameters.AddWithValue("@TableName", table.TableName);

                        using (var reader = latestCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var json = reader["DataSetUpdate"].ToString();
                                var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                var filtered = columns.ToDictionary(c => c, c => raw.ContainsKey(c) ? raw[c] : null);

                                var recordId = reader["RecordID"]?.ToString();
                                filtered["RecordID"] = recordId;

                                rows.Add(filtered);
                            }
                        }
                    }
                    else if (table.FormType == "T")
                    {
                        var allRowsCmd = new SqlCommand(@"
                        SELECT RecordID, DataSetUpdate 
                        FROM foProcessEventsDetail 
                        WHERE ProcessInstanceID = @InstanceID  
                          AND StepID = @StepID
                          AND TableName = @TableName
                          AND Active = 1 
                          AND ProcessEventID IN (
                              SELECT MAX(ProcessEventID) 
                              FROM foProcessEventsDetail 
                              WHERE ProcessInstanceID = @InstanceID 
                                AND StepID = @StepID
                                AND TableName = @TableName
                                AND Active = 1
                          )
                        ORDER BY ID", conn);

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

                                var recordId = reader["RecordID"]?.ToString();
                                filtered["RecordID"] = recordId;

                                rows.Add(filtered);
                            }
                        }
                    }


                    // Only if no data exists, add a blank row
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNewStepData(IFormCollection form, int userId, int stepId, int? processInstanceId, string action,  bool SendForApproval = false, List<int> SelectedApproverIds = null)
        {
            var insertedIds = new Dictionary<string, int>();
            var rowErrors = new List<string>();
            var baseAttachmentFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            Directory.CreateDirectory(baseAttachmentFolder);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Load step's table definitions
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

                        // Get or generate ProcessInstanceID
                        if (!processInstanceId.HasValue || processInstanceId == 0)
                        {
                            int? maxProcessInstanceId = null;

                            var tablesProcessInstanceID = new[]
                            {
                                "foProcessEvents",
                                "foProcessEventsArchive",
                            };

                            foreach (var table in tablesProcessInstanceID)
                            {
                                using (var cmdProcessInstanceID = new SqlCommand($"SELECT MAX(ProcessInstanceID) FROM {table}", conn, transaction))
                                {
                                    var result = cmdProcessInstanceID.ExecuteScalar();
                                    if (result != DBNull.Value && result != null)
                                    {
                                        var id = Convert.ToInt32(result);
                                        if (!maxProcessInstanceId.HasValue || id > maxProcessInstanceId.Value)
                                        {
                                            maxProcessInstanceId = id;
                                        }
                                    }
                                }
                            }

                            processInstanceId = (maxProcessInstanceId ?? 0) + 1;

                        }

                        // Insert initial event for this step
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

                        // Insert into all tables in order
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
                                            string descKey = $"desc_{tableName}_{colName}_{rowIndex}";
                                            string fileKey = $"file_{tableName}_{colName}_{rowIndex}";
                                            string desc = form.TryGetValue(descKey, out var d) ? d.ToString() : "";
                                            string file = form.Files.FirstOrDefault(f => f.Name == fileKey)?.FileName ?? "";
                                            value = !string.IsNullOrWhiteSpace(file) ? $"{desc};Attachments/{tableName}/{file}" : null;
                                        }

                                        parameterValues[colName] = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
                                        columnsSqlList.Add(colName);
                                        parametersSqlList.Add("@" + colName);
                                    }

                                    // FK resolution
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

                                    // Insert business record
                                    string insertSql = $"INSERT INTO {tableName} ({string.Join(", ", columnsSqlList)}) OUTPUT INSERTED.ID VALUES ({string.Join(", ", parametersSqlList)})";
                                    int newId;
                                    using (var insertCmd = new SqlCommand(insertSql, conn, transaction))
                                    {
                                        foreach (var col in columnsSqlList)
                                        {
                                            insertCmd.Parameters.AddWithValue("@" + col, parameterValues[col] ?? DBNull.Value);
                                        }
                                        newId = Convert.ToInt32(insertCmd.ExecuteScalar());
                                        insertedIds[$"{tableName}_{rowIndex}"] = newId;
                                    }

                                    // Insert into foProcessEventsDetail
                                    var rowSnapshot = parameterValues.ToDictionary(k => k.Key, k => k.Value);
                                    rowSnapshot["StepDescription"] = stepDescription;
                                    rowSnapshot["TableDescription"] = table.TableDescription;
                                    rowSnapshot["interactionType"] = "Insert";
                                    rowSnapshot["CreatedUserID"] = userId;
                                    rowSnapshot["CreatedDate"] = DateTime.Now;

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
                                    rowErrors.Add($"Error saving to table '{tableName}', row {rowIndex}: {ex.Message}");
                                }
                            }
                        }

                        if (rowErrors.Any())
                        {
                            transaction.Rollback();
                            TempData["RowErrors"] = rowErrors;
                            return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });
                        }

                        // Handle next step or approval transition
                        var transitionResult = HandleNextStep(conn, transaction, stepId, firstEventId, processInstanceId, userId, action, SendForApproval, SelectedApproverIds);
                                             

                        transaction.Commit();

                        return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        rowErrors.Add(ex.Message);
                        TempData["RowErrors"] = rowErrors;
                        TempData["Error"] = "An error occurred while saving step data.";
                        return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePendingStepData(IFormCollection form, int userId, int id, int stepId, int processInstanceId, string action, bool SendForApproval = false, List<int> SelectedApproverIds = null)
        {
            var insertedIds = new Dictionary<string, int>();
            var rowErrors = new List<string>();
            var baseAttachmentFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            Directory.CreateDirectory(baseAttachmentFolder);

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
                                        if (!columnDefs.Contains(colName, StringComparer.OrdinalIgnoreCase)) continue;

                                        var value = field.Value.ToString();
                                        parameterValues[colName] = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
                                        columnsSqlList.Add(colName);
                                        parametersSqlList.Add("@" + colName);
                                    }

                                    // FK logic
                                    if (!string.IsNullOrEmpty(fkColumn))
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

                        // ✅ Always save data first above
                        // ✅ Now handle what happens after save

                        //var transitionResult = HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId, action, form["cancelReason"]);
                        var transitionResult = HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId, action, SendForApproval, SelectedApproverIds, form["cancelReason"]);


                        transaction.Commit();

                        return RedirectToAction("MergedPendingItems", "PendingItems", new { userId });




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

            // 🔥 Handle cancellation first
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

            // 🔥 Always mark current event completed first
            using (var markDoneCmd = new SqlCommand(@"
        UPDATE foProcessEvents
        SET DateCompleted = GETDATE()
        WHERE ID = @EventID", conn, transaction))
            {
                markDoneCmd.Parameters.AddWithValue("@EventID", currentEventId);
                markDoneCmd.ExecuteNonQuery();
            }

            // 🔥 Try to find the next step
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
                // 🔥 Move to next step
                object assignedGroupId = nextGroupId;
                object assignedUserId = nextUserId;
                int stepToAssign = (int)nextStepId.Value;

                if (action == "SaveLater")
                {
                    assignedGroupId = DBNull.Value;
                    assignedUserId = userId;
                    stepToAssign = currentStepId; // Stay on current step
                }

                using (var insertNextCmd = new SqlCommand(@"
            INSERT INTO foProcessEvents
            (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
            VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE(), 1)", conn, transaction))
                {
                    insertNextCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertNextCmd.Parameters.AddWithValue("@StepID", stepToAssign);
                    insertNextCmd.Parameters.AddWithValue("@PreviousEventID", currentEventId);
                    insertNextCmd.Parameters.AddWithValue("@GroupID", assignedGroupId);
                    insertNextCmd.Parameters.AddWithValue("@UserID", assignedUserId);
                    insertNextCmd.ExecuteNonQuery();
                }

                result.NextStepExists = true;
            }
            else
            {
                // 🔥 No next step — check approvals
                if (SendForApproval && SelectedApproverIds?.Any() == true)
                {
                    // ➡️ Insert ad-hoc approvals into foApprovalEvents
                    foreach (var approverId in SelectedApproverIds)
                    {
                        using (var insertApprovalCmd = new SqlCommand(@"
                    INSERT INTO foApprovalEvents
                    (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
                    VALUES (@ProcessInstanceID, 0, @PreviousEventID, NULL, @UserID, GETDATE(), 1)", conn, transaction))
                        {
                            insertApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            insertApprovalCmd.Parameters.AddWithValue("@PreviousEventID", -currentEventId); // 🧠 Negative for rework tracking
                            insertApprovalCmd.Parameters.AddWithValue("@UserID", approverId);
                            insertApprovalCmd.ExecuteNonQuery();
                        }
                    }

                    result.ApprovalStarted = true;
                }
                else
                {
                    // 🔥 No ad-hoc approvals — check if predefined approvals exist
                    int processId;
                    using (var processIdCmd = new SqlCommand(@"
                SELECT ProcessID
                FROM foProcessSteps
                WHERE ID = @CurrentStepID", conn, transaction))
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



        private List<SelectListItem> GetForeignKeyOptions(string tableName)
        {
            var options = new List<SelectListItem>();
            string query = $"SELECT ID, Description FROM {tableName}";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            options.Add(new SelectListItem
                            {
                                Value = reader["ID"].ToString(),
                                Text = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }

            return options;
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

            // 🔥 Deactivate or delete original process events
            var deactivateCmd = new SqlCommand(@"
        DELETE FROM  foProcessEvents
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);

            deactivateCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deactivateCmd.ExecuteNonQuery();

            var deactivateDetailsCmd = new SqlCommand(@"
        DELETE FROM foProcessEventsDetail
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);

            deactivateDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deactivateDetailsCmd.ExecuteNonQuery();
        }


        private void PopulateProcessHistory(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            if (!processInstanceId.HasValue) return;

            var history = new List<ProcessEventAuditViewModel>();

            var cmd = new SqlCommand(@"
    SELECT ProcessEventID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo
FROM foProcessEventsDetail a LEFT OUTER JOIN foProcessSteps b on a.StepID = b.ID
WHERE ProcessInstanceID = @ProcessInstanceID
ORDER BY CreatedDate", conn, transaction);

            cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    history.Add(new ProcessEventAuditViewModel
                    {
                        ProcessEventID = Convert.ToInt32(reader["ProcessEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        Data = data
                    });
                }
            }

            // 🔹 APPROVAL EVENTS
            var approvalCmd = new SqlCommand(@"
        SELECT ApprovalEventID, StepID, 'foApproval' AS TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo
FROM foApprovalEventsDetail a LEFT OUTER JOIN foApprovalSteps b on a.StepID = b.ID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            approvalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            var approvalHistory = new List<ProcessEventAuditViewModel>();
            using (var reader = approvalCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    data["StepType"] = "Approval";

                    approvalHistory.Add(new ProcessEventAuditViewModel
                    {
                        ProcessEventID = Convert.ToInt32(reader["ApprovalEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
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
        public IActionResult ClaimStep(int id, int userId, int stepId, int processInstanceId, int processId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
        UPDATE foProcessEvents
        SET UserID = @UserID, GroupID = NULL
        WHERE ID = @id AND UserID IS NULL", conn);

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@id", id);
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



    }

}
