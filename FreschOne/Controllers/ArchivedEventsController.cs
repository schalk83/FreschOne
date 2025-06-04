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
    public class ArchivedEventsController : BaseController
    {
        public ArchivedEventsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }


        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult ArchivedStep(int Eventid, int processId, int? stepId, int? processInstanceId, int userId)
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
        
        private void PopulateProcessHistory(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            var ignoredColumns = GetIgnoredColumns(conn);
            if (!processInstanceId.HasValue) return;

            var history = new List<ProcessEventAuditViewModel>();

            var cmd = new SqlCommand(@"
        SELECT a.id AS DetailID, ProcessEventID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo, c.FirstName + ' '+ c.LastName AS FullName
        FROM foProcessEventsDetailArchive a 
        LEFT OUTER JOIN foProcessSteps b ON a.StepID = b.ID
        LEFT JOIN foUsers c ON c.ID = a.CreatedUserID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
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

            // 🔹 Approval Events
            var approvalCmd = new SqlCommand(@"
        SELECT a.id AS DetailID, ApprovalEventID, StepID, 'foApproval' AS TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, ISNULL(StepNo, '1.00') AS StepNo, c.FirstName + ' '+ c.LastName AS FullName
        FROM foApprovalEventsDetailArchive a 
        LEFT OUTER JOIN foApprovalSteps b ON a.StepID = b.ID
        LEFT JOIN foUsers c ON c.ID = a.CreatedUserID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            approvalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = approvalCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = rawData
                        .Where(kvp => !ignoredColumns.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    data["StepType"] = "Approval";

                    history.Add(new ProcessEventAuditViewModel
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

            // 🔹 Resolve FKs for All Tables
            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            var usedTables = history.Select(h => h.TableName).Distinct();

            foreach (var table in usedTables)
            {
                foreignKeys[table] = GetForeignKeyColumns(table);
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
                        if (record.Data.TryGetValue(fk.ColumnName, out var fkVal) && fkVal != null)
                        {
                            var fkId = fkVal.ToString();
                            var label = foreignKeyOptions[fk.TableName]
                                .FirstOrDefault(o => o.Value == fkId)?.Text;

                            if (!string.IsNullOrWhiteSpace(label))
                            {
                                record.Data[fk.ColumnName] = $"{label}";
                            }
                        }
                    }
                }

                // 🔥 Fallback FK resolution for non-`_md_` tables
                foreach (var kvp in record.Data.ToList())
                {
                    if (kvp.Key.EndsWith("ID", StringComparison.OrdinalIgnoreCase) && int.TryParse(kvp.Value?.ToString(), out var fkId))
                    {
                        var colName = kvp.Key;
                        var tableName = foreignKeys
                            .SelectMany(x => x.Value)
                            .FirstOrDefault(fk => fk.ColumnName == colName)?.TableName;

                        if (!string.IsNullOrEmpty(tableName))
                        {
                            var description = GetForeignKeyDescription(tableName, fkId.ToString());
                            if (!string.IsNullOrWhiteSpace(description))
                            {
                                record.Data[colName] = $"{description}";
                            }
                        }
                    }
                }

                // 🔹 Attachment Formatting
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
