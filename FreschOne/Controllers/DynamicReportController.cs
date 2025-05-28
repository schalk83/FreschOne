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
            string query = $"SELECT Description FROM {tableName} WHERE ID = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", foreignKeyValue);
                    description = command.ExecuteScalar()?.ToString();
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
