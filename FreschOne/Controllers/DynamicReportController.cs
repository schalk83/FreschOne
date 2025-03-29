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
        public DynamicReportController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

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
            var userReports = GetUserReports(userid);
            if (!userReports.Any()) return new List<foReports>();

            var reportIds = string.Join(",", userReports.Select(ur => ur.ReportID));
            var query = $"SELECT * FROM dbo.foReports WHERE ID IN ({reportIds}) AND Active = 1";
            return _dbHelper.ExecuteQuery<foReports>(query);
        }

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);

            var reports = GetReportsByUser(userid);
            ViewBag.userid = userid;
            return View(reports);
        }

        public IActionResult GenerateReport(int reportid, int PKID, int userId)
        {

            SetUserAccess(userId);
            string ReportName = GetReportName(reportid);
            ViewBag.ReportName = ReportName;
            ViewBag.ReportId = reportid;

            ViewBag.PKID = PKID;

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // ✅ Fetch all relevant tables for the report, including FKColumn
                string reportQuery = @"
            SELECT TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription
            FROM foReportTable 
            WHERE Active = 1 
            AND reportsid = " + reportid + 
            "ORDER BY Parent desc, ID "; // Parent comes first

                var tables = new List<foReportTable>();

                using (SqlCommand cmd = new SqlCommand(reportQuery, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new foReportTable
                            {
                                TableName = reader["TableName"].ToString(),
                                ColumnQuery = reader["ColumnQuery"].ToString(),
                                FormType = reader["FormType"].ToString(),
                                ColumnCount = reader["ColumnCount"] != DBNull.Value ? Convert.ToInt32(reader["ColumnCount"]) : (int?)null,
                                Parent = reader["Parent"] != DBNull.Value && Convert.ToBoolean(reader["Parent"]),
                                FKColumn = reader["FKColumn"]?.ToString(),
                                TableDescription = reader["TableDescription"]?.ToString()
                            });
                        }
                    }
                }

                // ✅ Fetch Table Prefixes for Cleaning Names
                var tablePrefixes = GetTablePrefixes();
                var tableDescriptions = new Dictionary<string, string>();

                foreach (var table in tables)
                {
                    string originalTableName = table.TableName;
                    string cleanedTableName = CleanTableName(originalTableName, tablePrefixes); // ✅ Clean table name
                    tableDescriptions[originalTableName] = cleanedTableName; // ✅ Store cleaned name
                }

                // ✅ Fetch Ignored Columns (Including FKColumn)
                var ignoredColumns = GetIgnoredColumns(conn);
                foreach (var table in tables)
                {
                    if (!string.IsNullOrEmpty(table.FKColumn))
                    {
                        ignoredColumns.Add(table.FKColumn); // ✅ Ensure FKColumn is excluded
                    }
                }

                // ✅ Fetch Foreign Keys for Lookup Tables Only
                var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
                foreach (var table in tables)
                {
                    foreignKeys[table.TableName] = GetForeignKeyColumns(table.TableName)
                        .Where(fk => fk.TableName.Contains("_md_")) // ✅ Only lookup tables (e.g., `tbl_md_*`)
                        .ToList();
                }

                // ✅ Fetch data for each table
                Dictionary<string, Queue<DataTable>> reportData = new Dictionary<string, Queue<DataTable>>();

                foreach (var table in tables)
                {
                    // ✅ Step 1: Get filtered columns
                    string filteredColumns = table.ColumnQuery.Trim() == "*"
                        ? GetAllColumnsExceptIgnored(table.TableName, ignoredColumns, conn)
                        : FilterIgnoredColumns(table.ColumnQuery, ignoredColumns);

                    // ✅ Step 2: Build NOT (col1 IS NULL AND col2 IS NULL ...) clause
                    var columnsForNullCheck = filteredColumns
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(col => col.Trim())
                        .Where(col =>
                            !string.Equals(col, "ID", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(col, "Active", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    string notAllNullClause = columnsForNullCheck.Count > 0
                        ? $"NOT ({string.Join(" AND ", columnsForNullCheck.Select(col => $"{col} IS NULL"))})"
                        : ""; // No clause if no valid columns

                    // ✅ Step 3: Build base WHERE clause
                    string foreignKeyColumn = table.FKColumn;
                    string baseWhereClause = (table.Parent == true)
                        ? $"ID = @PKID AND Active = 1"
                        : $"{foreignKeyColumn} = @PKID AND Active = 1";

                    // ✅ Step 4: Combine with NOT ALL NULL clause
                    if (!string.IsNullOrWhiteSpace(notAllNullClause))
                    {
                        baseWhereClause += $" AND {notAllNullClause}";
                    }

                    // ✅ Step 5: Final query
                    string selectQuery = $"SELECT {filteredColumns} FROM {table.TableName} WHERE {baseWhereClause} ORDER BY ID";

                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PKID", PKID);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // ✅ Convert DataTable into List of Dictionaries
                            var tableData = dt.Rows.Cast<DataRow>()
                                .Select(row => dt.Columns.Cast<DataColumn>()
                                    .ToDictionary(col => col.ColumnName, col => row[col])
                                ).ToList();

                            // ✅ Replace Foreign Key IDs with Descriptions **Only for Lookup Tables**
                            foreach (var rowDict in tableData)
                            {
                                foreach (var foreignKey in foreignKeys[table.TableName])
                                {
                                    if (rowDict.ContainsKey(foreignKey.ColumnName))
                                    {
                                        var fkValue = rowDict[foreignKey.ColumnName];

                                        if (fkValue != DBNull.Value)
                                        {
                                            rowDict[foreignKey.ColumnName] = GetForeignKeyDescription(foreignKey.TableName, fkValue);
                                        }
                                    }
                                }
                            }

                            // ✅ Convert List<Dictionary<string, object>> back to DataTable
                            DataTable updatedTable = ConvertToDataTable(tableData);

                            if (!reportData.ContainsKey(table.TableName))
                            {
                                reportData[table.TableName] = new Queue<DataTable>();
                            }

                            reportData[table.TableName].Enqueue(updatedTable);
                        }
                    }
                }

                // ✅ Store Results in ViewBag
                ViewBag.ReportTables = tables;
                ViewBag.ReportData = reportData;
                ViewBag.TableDescriptions = tableDescriptions;
                ViewBag.SearchResult = true;
            }

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


        //private string CleanTableName(string tableName, List<string> tablePrefixes)
        //{
        //    foreach (var prefix in tablePrefixes)
        //    {
        //        if (tableName.StartsWith(prefix))
        //        {
        //            return tableName.Replace(prefix, "").Replace("_", " "); // ✅ Remove prefix & format name
        //        }
        //    }
        //    return tableName.Replace("_", " "); // ✅ Default cleanup
        //}


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

    //public class foReportTable
    //{
    //    public string TableName { get; set; }
    //    public string TableDescription { get; set; }
    //    public string ColumnQuery { get; set; }
    //    public string FormType { get; set; } // "T" for Table, "F" for Freeform
    //    public int? ColumnCount { get; set; }
    //    public int? Parent { get; set; }
    //    public string FKColumn { get; set; }
    //    public List<ForeignKeyInfo> ForeignKeys { get; set; }  // List of foreign key columns (for dropdowns)
    //    public Dictionary<string, List<SelectListItem>> ForeignKeyOptions { get; set; }  // Options for foreign key dropdowns

    //}
}
