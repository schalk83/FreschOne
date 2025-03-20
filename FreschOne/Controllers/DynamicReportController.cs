using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class DynamicReportController : BaseController
    {
        public DynamicReportController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
      
            public IActionResult Index(int userId)
            {
                ViewBag.SearchResult = null; // Default, nothing visible
                return View();
            }

        [HttpPost]
        public IActionResult GenerateReport(int parentID)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string reportQuery = @"SELECT TableName, Colunms, FormType, ColumnCount, Parent
                               FROM foReportTable 
                               WHERE Active = 1 
                               ORDER BY Parent DESC"; // Parent comes first

                var tables = new List<ReportTable>();

                using (SqlCommand cmd = new SqlCommand(reportQuery, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new ReportTable
                            {
                                TableName = reader["TableName"].ToString(),
                                Columns = reader["Colunms"].ToString(),
                                FormType = reader["FormType"].ToString(),
                                ColumnCount = reader["ColumnCount"] != DBNull.Value ? Convert.ToInt32(reader["ColumnCount"]) : (int?)null,
                                Parent = reader["Parent"] != DBNull.Value ? Convert.ToInt32(reader["Parent"]) : (int?)null
                            });
                        }
                    }
                }

                // ✅ Fetch Table Prefixes
                var tablePrefixes = GetTablePrefixes();
                var tableDescriptions = new Dictionary<string, string>();

                foreach (var table in tables)
                {
                    string originalTableName = table.TableName;
                    string cleanedTableName = CleanTableName(originalTableName, tablePrefixes); // ✅ Clean table name
                    tableDescriptions[originalTableName] = cleanedTableName; // ✅ Store cleaned name
                }

                // ✅ Fetch Columns to Ignore
                var ignoredColumns = GetIgnoredColumns(conn);

                // ✅ Fetch data for each table
                Dictionary<string, Queue<DataTable>> reportData = new Dictionary<string, Queue<DataTable>>();

                foreach (var table in tables)
                {
                    string foreignKeyColumn = null;

                    // ✅ If Columns is "*", get actual column names
                    string filteredColumns = table.Columns.Trim() == "*"
                        ? GetAllColumnsExceptIgnored(table.TableName, ignoredColumns, conn)
                        : FilterIgnoredColumns(table.Columns, ignoredColumns);

                    string selectQuery;
                    if (table.Parent == 1) // Parent Table → Filter by Primary Key
                    {
                        selectQuery = $"SELECT {filteredColumns} FROM {table.TableName} WHERE ID = @ParentId";
                    }
                    else // Child Tables → Dynamically determine foreign key (always second column)
                    {
                        foreignKeyColumn = GetSecondColumnName(table.TableName, conn);
                        selectQuery = $"SELECT {filteredColumns} FROM {table.TableName} WHERE {foreignKeyColumn} = @ParentId";
                    }

                    using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ParentId", parentID);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (!reportData.ContainsKey(table.TableName))
                            {
                                reportData[table.TableName] = new Queue<DataTable>();
                            }

                            reportData[table.TableName].Enqueue(dt);
                        }
                    }
                }

                ViewBag.ReportTables = tables;
                ViewBag.ReportData = reportData;
                ViewBag.TableDescriptions = tableDescriptions; // ✅ Store cleaned table names
                ViewBag.SearchResult = true;
            }

            return View("Index");
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
            string cleanedName = tableName;

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


        private string CleanTableName(string tableName, List<string> tablePrefixes)
        {
            foreach (var prefix in tablePrefixes)
            {
                if (tableName.StartsWith(prefix))
                {
                    return tableName.Replace(prefix, "").Replace("_", " "); // ✅ Remove prefix & format name
                }
            }
            return tableName.Replace("_", " "); // ✅ Default cleanup
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

    public class ReportTable
    {
        public string TableName { get; set; }

        public string TableDescription { get; set; }
        public string Columns { get; set; }
        public string FormType { get; set; } // "T" for Table, "F" for Freeform
        public int? ColumnCount { get; set; }
        public int? Parent { get; set; }
    }
}
