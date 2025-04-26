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

                // ... existing logic ...
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

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                var ignoredColumns = GetIgnoredColumns(conn);

                var query = @"SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
                      FROM foProcessDetail 
                      WHERE StepID = @StepID AND Active = 1
                      ORDER BY Parent DESC, ID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
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

                    // 🧠 Get metadata for the table columns
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

            return View();
        }

        public IActionResult PendingStep(int processId, int? stepId, int? processInstanceId, int userId)
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

                // 🔍 Fetch process step tables
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

                    ///here! AND StepID = @StepID
                    var latestCmd = new SqlCommand(@"
                SELECT RecordID, DataSetUpdate 
                FROM foProcessEventsDetail 
                WHERE ProcessInstanceID = @InstanceID  AND TableName = @TableName AND Active = 1 
                ORDER BY ID", conn);
                    latestCmd.Parameters.AddWithValue("@InstanceID", processInstanceId);
                    latestCmd.Parameters.AddWithValue("@TableName", table.TableName);

                    var rows = new List<Dictionary<string, object>>();
                    using (var reader = latestCmd.ExecuteReader())
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

                    // If no rows, at least show one empty row
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveNewStepData(IFormCollection form, int userId, int stepId, int? processInstanceId)
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
                            string getMaxInstanceIdSql = @"
                        SELECT ISNULL(MAX(ProcessInstanceID), 0) + 1 
                        FROM (
                            SELECT ProcessInstanceID FROM foProcessEvents
                            UNION ALL
                            SELECT ProcessInstanceID FROM foApprovalEventsArchive
                        ) AS Combined";
                            using (var getInstanceCmd = new SqlCommand(getMaxInstanceIdSql, conn, transaction))
                            {
                                processInstanceId = Convert.ToInt32(getInstanceCmd.ExecuteScalar());
                            }
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

                                    // FK resolution (new logic)
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

                                    // Add system fields
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

                                    // Snapshot to foProcessEventsDetail
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
                            return RedirectToAction("ExecuteStep", new { stepId, processInstanceId, userId });
                        }

                        // Route to next step or approval
                        string nextStepQuery = @"SELECT TOP 1 ID, GroupID, UserID 
                                         FROM foProcessSteps 
                                         WHERE ProcessID = (SELECT ProcessID FROM foProcessSteps WHERE ID = @StepID) 
                                         AND StepNo > (SELECT StepNo FROM foProcessSteps WHERE ID = @StepID) 
                                         ORDER BY StepNo";
                        long? nextStepId = null;
                        object nextGroupId = DBNull.Value, nextUserId = DBNull.Value;

                        using (var getNextCmd = new SqlCommand(nextStepQuery, conn, transaction))
                        {
                            getNextCmd.Parameters.AddWithValue("@StepID", stepId);
                            using var reader = getNextCmd.ExecuteReader();
                            if (reader.Read())
                            {
                                nextStepId = reader.GetInt64(0);
                                nextGroupId = reader["GroupID"];
                                nextUserId = reader["UserID"];
                            }
                        }

                        if (nextStepId.HasValue)
                        {
                            using var nextCmd = new SqlCommand(@"
                        INSERT INTO foProcessEvents (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
                        VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE(), 1)", conn, transaction);
                            nextCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            nextCmd.Parameters.AddWithValue("@StepID", nextStepId);
                            nextCmd.Parameters.AddWithValue("@PreviousEventID", firstEventId);
                            nextCmd.Parameters.AddWithValue("@GroupID", nextGroupId);
                            nextCmd.Parameters.AddWithValue("@UserID", nextUserId);
                            nextCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            var markCompleteCmd = new SqlCommand("UPDATE foProcessEvents SET DateCompleted = GETDATE() WHERE ID = @EventID", conn, transaction);
                            markCompleteCmd.Parameters.AddWithValue("@EventID", firstEventId);
                            markCompleteCmd.ExecuteNonQuery();

                            var getProcessIdCmd = new SqlCommand("SELECT ProcessID FROM foProcessSteps WHERE ID = @StepID", conn, transaction);
                            getProcessIdCmd.Parameters.AddWithValue("@StepID", stepId);
                            int processId = Convert.ToInt32(getProcessIdCmd.ExecuteScalar());

                            var approvalStepCmd = new SqlCommand(@"
                        SELECT TOP 1 ID, GroupID, UserID 
                        FROM foApprovalSteps 
                        WHERE ProcessID = @ProcessID AND Active = 1 
                        ORDER BY StepNo", conn, transaction);
                            approvalStepCmd.Parameters.AddWithValue("@ProcessID", processId);

                            using var reader = approvalStepCmd.ExecuteReader();
                            if (reader.Read())
                            {
                                int approvalStepId = Convert.ToInt32(reader["ID"]);
                                var approvalGroupId = reader["GroupID"];
                                var approvalUserId = reader["UserID"];

                                var insertApproval = new SqlCommand(@"
                            INSERT INTO foApprovalEvents (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned)
                            VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE())", conn, transaction);
                                insertApproval.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                insertApproval.Parameters.AddWithValue("@StepID", approvalStepId);
                                insertApproval.Parameters.AddWithValue("@PreviousEventID", -firstEventId);
                                insertApproval.Parameters.AddWithValue("@GroupID", approvalGroupId ?? DBNull.Value);
                                insertApproval.Parameters.AddWithValue("@UserID", approvalUserId ?? DBNull.Value);
                                insertApproval.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        return nextStepId.HasValue
                            ? RedirectToAction("PendingItems", new { userId })
                            : RedirectToAction("PendingApprovalItems", "ApprovalEvents", new { userId });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        rowErrors.Add(ex.Message);
                        TempData["RowErrors"] = rowErrors;
                        TempData["Error"] = "An error occurred while saving step data.";
                        return RedirectToAction("ExecuteStep", new { stepId, processInstanceId, userId });
                    }
                }
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePendingStepData(IFormCollection form, int userId, int stepId, int processInstanceId)
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

                        using (var updateCmd = new SqlCommand("UPDATE foProcessEvents SET DateCompleted = GETDATE() WHERE ID = @ID", conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@ID", processEventId);
                            updateCmd.ExecuteNonQuery();
                        }

                        int? rootParentId = null;
                        string rootParentQuery = @"
                    SELECT TOP 1 RecordID FROM foProcessEventsDetail 
                    WHERE ProcessInstanceID = @ProcessInstanceID 
                    AND StepID = (SELECT MIN(StepID) FROM foProcessEvents WHERE ProcessInstanceID = @ProcessInstanceID)
                    AND TableName IN (SELECT TableName FROM foProcessDetail WHERE Parent = 1 AND FKColumn IS NULL)";
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
                                    parameterValues["ModifiedUserID"] = userId;
                                    parameterValues["ModifiedDate"] = DateTime.Now;

                                    if (formType == "F" && recordId > 0)
                                    {
                                        // 🔥 UPDATE instead of insert for Freeform if record exists
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
                                        // INSERT (for Tabular, or new Freeform record)
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

                        // ➕ Handle next step creation
                        HandleNextStep(conn, transaction, stepId, processEventId, processInstanceId, userId);

                        transaction.Commit();
                        return RedirectToAction("PendingItems", new { userId });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["RowErrors"] = new List<string> { ex.Message };
                        return RedirectToAction("ExecuteStep", new { stepId, processInstanceId, userId });
                    }
                }
            }
        }

        private void HandleNextStep(SqlConnection conn, SqlTransaction transaction, int currentStepId, int currentEventId, int processInstanceId, int userId)
        {
            // ➡️ Find next normal step
            string nextStepQuery = @"
        SELECT TOP 1 ID, GroupID, UserID 
        FROM foProcessSteps 
        WHERE ProcessID = (SELECT ProcessID FROM foProcessSteps WHERE ID = @CurrentStepID) 
        AND StepNo > (SELECT StepNo FROM foProcessSteps WHERE ID = @CurrentStepID) 
        ORDER BY StepNo";

            long? nextStepId = null;
            object nextGroupId = DBNull.Value, nextUserId = DBNull.Value;

            using (var getNextCmd = new SqlCommand(nextStepQuery, conn, transaction))
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
                // ➡️ Create next process event (normal step)
                using (var insertNextCmd = new SqlCommand(@"
            INSERT INTO foProcessEvents 
            (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
            VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE(), 1)", conn, transaction))
                {
                    insertNextCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertNextCmd.Parameters.AddWithValue("@StepID", nextStepId.Value);
                    insertNextCmd.Parameters.AddWithValue("@PreviousEventID", currentEventId);
                    insertNextCmd.Parameters.AddWithValue("@GroupID", nextGroupId);
                    insertNextCmd.Parameters.AddWithValue("@UserID", nextUserId);
                    insertNextCmd.ExecuteNonQuery();
                }
            }
            else
            {
                // 🚀 No next step ➡️ Complete and move to Approval phase
                using (var markDoneCmd = new SqlCommand(
                    "UPDATE foProcessEvents SET DateCompleted = GETDATE() WHERE ID = @EventID", conn, transaction))
                {
                    markDoneCmd.Parameters.AddWithValue("@EventID", currentEventId);
                    markDoneCmd.ExecuteNonQuery();
                }

                int processId;
                using (var processIdCmd = new SqlCommand(
                    "SELECT ProcessID FROM foProcessSteps WHERE ID = @StepID", conn, transaction))
                {
                    processIdCmd.Parameters.AddWithValue("@StepID", currentStepId);
                    processId = Convert.ToInt32(processIdCmd.ExecuteScalar());
                }

                using (var approvalStepCmd = new SqlCommand(@"
            SELECT TOP 1 ID, GroupID, UserID 
            FROM foApprovalSteps 
            WHERE ProcessID = @ProcessID AND Active = 1 
            ORDER BY StepNo", conn, transaction))
                {
                    approvalStepCmd.Parameters.AddWithValue("@ProcessID", processId);
                    using (var reader = approvalStepCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var approvalStepId = Convert.ToInt32(reader["ID"]);
                            var approvalGroupId = reader["GroupID"] ?? DBNull.Value;
                            var approvalUserId = reader["UserID"] ?? DBNull.Value;

                            using (var insertApproval = new SqlCommand(@"
                        INSERT INTO foApprovalEvents 
                        (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned)
                        VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @GroupID, @UserID, GETDATE())", conn, transaction))
                            {
                                insertApproval.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                insertApproval.Parameters.AddWithValue("@StepID", approvalStepId);
                                insertApproval.Parameters.AddWithValue("@PreviousEventID", -currentEventId); // 🔥 Negative for tracking
                                insertApproval.Parameters.AddWithValue("@GroupID", approvalGroupId);
                                insertApproval.Parameters.AddWithValue("@UserID", approvalUserId);
                                insertApproval.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
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
                RowCount = model.CurrentRowCount + 1,
                RenderRowIndex = model.CurrentRowCount,
                ColumnTypes = columnTypes,
                ColumnLengths = columnLengths,
                ForeignKeys = foreignKeys,
                ForeignKeyOptions = foreignKeyOptions
            };

            return PartialView("_TableRows", vm);
        }




        ///PENDING
        ///
        public IActionResult PendingItems(int userId)
        {
            SetUserAccess(userId);
            var pendingSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"
            SELECT 
                e.ID,
                e.ProcessInstanceID,
                e.StepID,
                e.GroupID,
                g.Description AS GroupDescription,
                e.UserID,
                u.FirstName,
                u.LastName,
                e.DateAssigned,
                s.StepDescription,
                s.StepNo,
                s.ProcessID
            FROM foProcessEvents e
            JOIN foProcessSteps s ON e.StepID = s.ID 
            LEFT JOIN foGroups g ON e.GroupID = g.ID
            LEFT JOIN foUsers u ON e.UserID = u.ID
            WHERE e.DateCompleted IS NULL
              AND (e.UserID = @UserID 
                   OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
            ORDER BY e.DateAssigned DESC";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7)
                                    ? null
                                    : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.GetDateTime(8),
                                StepDescription = reader.GetString(9),
                                StepNo = Convert.ToDouble(reader["StepNo"]),
                                ProcessID = Convert.ToInt32(reader["ProcessID"])
                            });
                        }
                    }
                }
            }

            ViewBag.UserID = userId;
            return View(pendingSteps);
        }


        [HttpPost]
        public IActionResult ClaimStep(int eventId, int userId, int stepId, int processInstanceId, int processId)
        {
            using var conn = GetConnection();
            conn.Open();

            using var cmd = new SqlCommand(@"
        UPDATE foProcessEvents
        SET UserID = @UserID, GroupID = NULL
        WHERE ID = @EventID AND UserID IS NULL", conn);

            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@EventID", eventId);
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




    }

}
