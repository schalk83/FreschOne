using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class DynamicReportController : BaseController
    {
        public DynamicReportController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult GenerateReportIndex(int userId)
        {
            SetUserAccess(userId);

            ViewBag.SearchResult = null; // Default, nothing visible
            ViewBag.userID = userId;
            return View();
        }

        private List<foUserReports> GetUserReports(int userid)
        {
            var query = "SELECT * FROM dbo.foUserReports WHERE UserID = @UserID AND Active = 1";
            return _dbHelper.ExecuteQuery<foUserReports>(query, new { UserID = userid });
        }

        private List<foReports> GetReportsByUser(int userid)
        {
            var accessibleReportIds = new List<long>();

            using (var conn = GetConnection()) // Or _dbHelper.GetConnection()
            {
                conn.Open();

                // 1️⃣ Get user's group IDs
                var userGroups = new List<long>();
                var groupCmd = new SqlCommand("SELECT GroupID FROM foUserGroups WHERE UserID = @UserID AND Active = 1", conn);
                groupCmd.Parameters.AddWithValue("@UserID", userid);
                using (var reader = groupCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userGroups.Add((long)reader["GroupID"]);
                    }
                }

                // 2️⃣ Get accessible ReportIDs from foReportAccess
                var groupIdList = userGroups.Any() ? string.Join(",", userGroups) : "NULL";
                var reportCmd = new SqlCommand($@"
            SELECT DISTINCT ReportID
            FROM foReportAccess
            WHERE Active = 1
              AND ReportID IS NOT NULL
              AND (
                   UserID = @UserID
                   OR (GroupID IS NOT NULL AND GroupID IN ({groupIdList}))
              )", conn);
                reportCmd.Parameters.AddWithValue("@UserID", userid);

                using (var reader = reportCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessibleReportIds.Add((long)reader["ReportID"]);
                    }
                }
            }

            if (!accessibleReportIds.Any()) return new List<foReports>();

            var reportIds = string.Join(",", accessibleReportIds);

            var combinedQuery = $@"
        SELECT ID, ReportName, ReportDescription, Active, 'Table' AS Source
        FROM dbo.foReports
        WHERE ID IN ({reportIds})
          AND Active = 1
          AND ID IN (SELECT ReportsID FROM foReportTable WHERE Active = 1)

        UNION ALL

        SELECT ID, ReportName, ReportDescription, Active, 'Query' AS Source
        FROM dbo.foReports
        WHERE ID IN ({reportIds})
          AND Active = 1
          AND ID IN (SELECT ReportsID FROM foReportTableQuery WHERE Active = 1)
    ";

            return _dbHelper.ExecuteQuery<foReports>(combinedQuery);
        }



        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);

            var reports = GetReportsByUser(userid);
            ViewBag.userid = userid;
            return View(reports);
        }

        public IActionResult GenerateReport(int reportid, int? PKID, int userId)
        {
            SetUserAccess(userId);
            ViewBag.ReportName = GetReportName(reportid);
            ViewBag.ReportId = reportid;
            ViewBag.PKID = PKID;

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            var reportData = new Dictionary<string, Queue<DataTable>>();
            var allTables = new List<object>();
            var tableDescriptions = new Dictionary<string, string>();
            var tablesWithQR = new HashSet<string>(); // 🌟 Track which tables need QR

            using var conn = new SqlConnection(connStr);
            conn.Open();

            var ignoredColumns = GetIgnoredColumns(conn);
            var tablePrefixes = GetTablePrefixes();

            // 🔷 Load foReportTable
            var foTables = new List<foReportTable>();
            using (var cmd = new SqlCommand(@"
        SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
        FROM foReportTable 
        WHERE Active = 1 AND ReportsID = @ReportID
        ORDER BY Parent DESC, ID", conn))
            {
                cmd.Parameters.AddWithValue("@ReportID", reportid);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var table = new foReportTable
                    {
                        TableName = reader["TableName"].ToString(),
                        ColumnQuery = reader["ColumnQuery"].ToString(),
                        FormType = reader["FormType"].ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        Parent = Convert.ToBoolean(reader["Parent"]),
                        FKColumn = reader["FKColumn"]?.ToString(),
                        TableDescription = reader["TableDescription"]?.ToString()
                    };

                    // 🌟 Check for QR in ColumnQuery
                    if (table.ColumnQuery?.IndexOf("'QR' as QR", StringComparison.OrdinalIgnoreCase) >= 0)
                        tablesWithQR.Add(table.TableName);
                        

                    foTables.Add(table);
                    allTables.Add(table);
                    tableDescriptions[table.TableName] = CleanTableName(table.TableName, tablePrefixes);
                }
            }

            // 🔷 Load foReportTableQuery
            var foQueryTables = new List<foReportTableQuery>();
            using (var cmd = new SqlCommand(@"
        SELECT ID, Query, FormType, ColumnCount, TableDescription
        FROM foReportTableQuery
        WHERE Active = 1 AND ReportsID = @ReportID
        ORDER BY ID", conn))
            {
                cmd.Parameters.AddWithValue("@ReportID", reportid);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var qtable = new foReportTableQuery
                    {
                        ID = (long)reader["ID"],
                        Query = reader["Query"].ToString(),
                        FormType = reader["FormType"].ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        TableDescription = reader["TableDescription"]?.ToString()
                    };

                    // 🌟 Check for QR in Query
                    if (qtable.Query?.IndexOf("QR", StringComparison.OrdinalIgnoreCase) >= 0)
                        tablesWithQR.Add($"Q_{qtable.ID}");

                    foQueryTables.Add(qtable);
                    allTables.Add(qtable);
                    tableDescriptions[$"Q_{qtable.ID}"] = $"🧪 {qtable.TableDescription ?? "Custom Query"}";
                }
            }

            // 🔷 Execute Query Tables
            foreach (var qtable in foQueryTables)
            {
                try
                {
                    using var customCmd = new SqlCommand(qtable.Query, conn);
                    if (qtable.Query.Contains("@PKID") && PKID.HasValue)
                        customCmd.Parameters.AddWithValue("@PKID", PKID.Value);

                    var dt = new DataTable();
                    new SqlDataAdapter(customCmd).Fill(dt);

                    reportData[$"Q_{qtable.ID}"] = new Queue<DataTable>(new[] { dt });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Query {qtable.ID} failed: {ex.Message}");
                }
            }

            string cols; 
            // 🔷 Execute Structured Tables
            foreach (var table in foTables)
            {
                List<string> columnParts;

                if (table.ColumnQuery.Trim() == "*")
                {
                    cols = GetAllColumnsExceptIgnored(table.TableName, ignoredColumns, conn);
                    columnParts = cols.Split(',').Select(c => c.Trim()).ToList();
                }
                else
                {
                    columnParts = table.ColumnQuery.Split(',')
                        .Select(c => c.Trim())
                        .Where(c => !ignoredColumns.Contains(c, StringComparer.OrdinalIgnoreCase))
                        .ToList();

                    cols = string.Join(",", columnParts);
                }

                string where = table.Parent
                    ? "ID = @PKID AND Active = 1"
                    : $"{table.FKColumn} = @PKID AND Active = 1";

                string nullFilter = string.Join(" AND ", columnParts
                    .Where(c => !c.StartsWith("'") && !c.Contains(" as ", StringComparison.OrdinalIgnoreCase) && c != "ID" && c != "Active")
                    .Select(c => $"{c} IS NULL"));

                if (!string.IsNullOrWhiteSpace(nullFilter))
                    where += $" AND NOT ({nullFilter})";

                string query = $"SELECT {cols} FROM {table.TableName} WHERE {where} ORDER BY ID";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PKID", PKID ?? 0);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                if (!reportData.ContainsKey(table.TableName))
                    reportData[table.TableName] = new Queue<DataTable>();
                reportData[table.TableName].Enqueue(dt);
            }

            // 🔁 Enhance reportData: Resolve FK descriptions and foUserID_ names
            var foreignKeyCache = new Dictionary<string, List<ForeignKeyInfo>>();
            var fkOptionCache = new Dictionary<string, List<SelectListItem>>();
            var userMap = new Dictionary<string, string>();

            // Preload foUsers
            using (var userCmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn))
            using (var reader = userCmd.ExecuteReader())
            {
                while (reader.Read())
                    userMap[reader["ID"].ToString()] = reader["FullName"].ToString();
            }

            // Enhance each DataTable in reportData
            foreach (var key in reportData.Keys.ToList())
            {
                var queue = reportData[key];
                var newQueue = new Queue<DataTable>();

                while (queue.Count > 0)
                {
                    var dt = queue.Dequeue();
                    var enhancedDt = dt.Clone();

                    foreach (DataColumn col in dt.Columns)
                    {
                        string colName = col.ColumnName;
                        bool isFoUser = colName.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase);
                        string displayCol = $"{colName}_Display";
                        enhancedDt.Columns.Add(displayCol, typeof(string));
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        var newRow = enhancedDt.NewRow();
                        newRow.ItemArray = row.ItemArray.Concat(Enumerable.Repeat<object>("", dt.Columns.Count)).ToArray();

                        foreach (DataColumn col in dt.Columns)
                        {
                            string colName = col.ColumnName;
                            string rawValue = row[colName]?.ToString();
                            string displayCol = $"{colName}_Display";

                            if (colName.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrWhiteSpace(rawValue) && userMap.TryGetValue(rawValue, out var fullName))
                                {
                                    newRow[displayCol] = fullName;
                                }
                            }
                            else if (colName.EndsWith("ID", StringComparison.OrdinalIgnoreCase) && colName != "ID")
                            {
                                string fkTable;
                                if (!foreignKeyCache.ContainsKey(key))
                                    foreignKeyCache[key] = GetForeignKeyColumns(key);

                                var fk = foreignKeyCache[key].FirstOrDefault(f => f.ColumnName == colName);
                                if (fk != null)
                                {
                                    fkTable = fk.TableName;
                                    if (!fkOptionCache.ContainsKey(fkTable))
                                        fkOptionCache[fkTable] = GetForeignKeyOptions(fkTable);

                                    var match = fkOptionCache[fkTable].FirstOrDefault(opt => opt.Value == rawValue);
                                    if (match != null)
                                        newRow[displayCol] = match.Text;
                                }
                            }
                        }

                        enhancedDt.Rows.Add(newRow);
                    }

                    newQueue.Enqueue(enhancedDt);
                }

                reportData[key] = newQueue;
            }


            // 🔷 Pass Flags to View
            ViewBag.ReportTables = allTables;
            ViewBag.ReportData = reportData;
            ViewBag.TableDescriptions = tableDescriptions;
            ViewBag.TablesWithQR = tablesWithQR; // 🌟 New
            ViewBag.SearchResult = true;

            return View("GenerateReportIndex");
        }

        private DataTable ConvertToDataTable(List<Dictionary<string, object>> data)
        {
            DataTable table = new DataTable();

            if (data.Count == 0)
                return table;

            // ✅ Create columns
            foreach (var column in data[0].Keys)
            {
                table.Columns.Add(column);
            }

            // ✅ Add rows
            foreach (var rowDict in data)
            {
                var row = table.NewRow();
                foreach (var column in rowDict.Keys)
                {
                    row[column] = rowDict[column] ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

        private string GetForeignKeyDescription(string tableName, object foreignKeyValue)
        {
            string description = string.Empty;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Check if table is a Maintenance table (foTablePrefixes.Description = 'Maintenance')
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM foTablePrefixes 
            WHERE @TableName LIKE Prefix + '%' AND Description = 'Maintenance'";

                bool isMaintenance = false;
                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@TableName", tableName);
                    isMaintenance = (int)checkCmd.ExecuteScalar() > 0;
                }

                if (isMaintenance)
                {
                    // Fetch the Description column for Maintenance tables
                    string descQuery = $"SELECT Description FROM {tableName} WHERE ID = @Id";
                    using (var cmd = new SqlCommand(descQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", foreignKeyValue);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            description = result.ToString();
                        }
                    }
                }
                else
                {
                    // Fetch ALL non-ID columns (excluding ignored ones)
                    var columns = new List<string>();
                    var columnsQuery = @"
                    SELECT COLUMN_NAME, DATA_TYPE 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName 
                    AND COLUMN_NAME NOT IN ('ID', 'Active') 
                    AND COLUMN_NAME NOT LIKE '%ID'";

                    using (var cmd = new SqlCommand(columnsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["COLUMN_NAME"].ToString();
                                string dataType = reader["DATA_TYPE"].ToString().ToLower();

                                // If the column is not varchar, nvarchar, or text, convert it to string
                                if (dataType != "varchar" && dataType != "nvarchar" && dataType != "text")
                                {
                                    columnName = $"CONVERT(varchar, {columnName})";
                                }

                                columns.Add(columnName);
                            }
                        }
                    }

                    if (columns.Count > 0)
                    {
                        var selectClause = string.Join(" + ' | ' + ", columns);
                        var compositeQuery = $"SELECT {selectClause} FROM {tableName} WHERE ID = @Id";

                        using (var cmd = new SqlCommand(compositeQuery, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", foreignKeyValue);
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                description = result.ToString();
                            }
                        }
                    }

                }
            }

            return description;
        }


        private string GetReportName(int reportid)
        {
            string ReportDescription = string.Empty;
            string query = $"SELECT ReportName FROM foReports WHERE ID = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", reportid);
                    ReportDescription = command.ExecuteScalar()?.ToString();
                }
            }

            return ReportDescription;
        }

        private List<ForeignKeyInfo> GetForeignKeyColumns(string tablename)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            string query = @"
        SELECT 
            c.name AS ColumnName,
            ref_tab.name AS ReferencedTableName,
	        CASE WHEN right ( c.name, 2) = 'ID' THEN LEFT ( c.name, LEN ( c.name ) -2 ) ELSE c.NAME END AS ColumnDescription
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
            parent_tab.name = @TableName
            AND c.name LIKE '%ID' ";

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
                                TableName = reader.GetString(1), // Reference to the related 'tbl_md_*' table
                                ColumnDescription = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return foreignKeys;
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

        private string GetAllColumnsExceptIgnored(string tableName, List<string> ignoredColumns, SqlConnection conn)
        {
            var columnNames = new List<string>();

            string query = @"
        SELECT COLUMN_NAME 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = @TableName";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        if (!ignoredColumns.Contains(columnName)) // ✅ Exclude ignored columns
                        {
                            columnNames.Add(columnName);
                        }
                    }
                }
            }

            return columnNames.Any() ? string.Join(", ", columnNames) : "*"; // ✅ Avoid empty column selection
        }



        private string FilterIgnoredColumns(string columns, List<string> ignoredColumns)
        {
            var columnList = columns.Split(',').Select(c => c.Trim()).ToList();
            columnList.RemoveAll(col => ignoredColumns.Contains(col)); // ✅ Remove ignored columns

            return columnList.Any() ? string.Join(", ", columnList) : "*"; // ✅ Avoid empty selection
        }

        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            var prefixes = _dbHelper.ExecuteQuery<foTablePrefix>(query);
            return prefixes;
        }

        private string CleanTableName(string tableName, List<foTablePrefix> tablePrefixes)
        {
            string cleanedName = tableName.ToLower();

            foreach (var prefix in tablePrefixes)
            {
                if (cleanedName.StartsWith(prefix.Prefix)) // ✅ Check if name starts with prefix
                {
                    cleanedName = cleanedName.Replace(prefix.Prefix, ""); // ✅ Remove prefix
                    break; // ✅ Stop after first match
                }
            }

            return cleanedName.Replace("_", " "); // ✅ Replace underscores with spaces
        }

        // Helper method to get the second column of a table
        private string GetSecondColumnName(string tableName, SqlConnection conn)
        {
            string secondColumn = null;
            string query = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @TableName 
                ORDER BY ORDINAL_POSITION OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", tableName);
                secondColumn = cmd.ExecuteScalar()?.ToString();
            }

            return secondColumn ?? throw new Exception($"No second column found for table {tableName}");
        }


    }

 
}
