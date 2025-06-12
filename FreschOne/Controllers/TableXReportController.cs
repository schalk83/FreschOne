using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace FreschOne.Controllers
{
    public class TableXReport : BaseController
    {
        public TableXReport(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult preIndex(int userid, int reportid)
        {
            string tablename = string.Empty;
            string query = "SELECT tablename FROM dbo.foReportTable WHERE ReportsID = @reportid AND Parent = 1";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@reportid", reportid);
                    tablename = command.ExecuteScalar()?.ToString() ?? "";
                }
            }

            // Redirect to Index, passing tablename as a parameter
            return RedirectToAction("Index", new { userid, reportid, tablename });
        }


        public IActionResult Index(int userid, int reportid, string tablename, int pageNumber = 1, string searchText = "")
        {
            EnsureAuditFieldsExist(tablename);
            SetUserAccess(userid);

            ViewBag.ReportName = GetReportName(reportid);
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ReportID = reportid;

            var tablePrefixes = GetTablePrefixes();
            var tableDescription = tablePrefixes
                .Where(p => tablename.StartsWith(p.Prefix))
                .Select(p => tablename.Replace(p.Prefix, ""))
                .FirstOrDefault() ?? tablename;
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            bool hasQR;
            var columns = GetTableColumns(tablename, reportid, out hasQR);
            ViewBag.HasQR = hasQR;

            var tableData = GetTableData(tablename, reportid, hasQR);

            if (!string.IsNullOrEmpty(searchText))
            {
                tableData = tableData
                    .Where(row => row.Values.Any(v => v?.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();
            }

            var foreignKeys = GetForeignKeyColumns(tablename);

            var foUserKeys = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c))
                .Select(c => new ForeignKeyInfo
                {
                    ColumnName = c,
                    TableName = "foUsers",
                    ColumnDescription = "User"
                }).ToList();

            foreignKeys.AddRange(foUserKeys);

            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys.Where(f => f.TableName != "foUsers"))
            {
                if (!fkOptions.ContainsKey(fk.TableName))
                {
                    fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                }
            }

            var userMap = new Dictionary<string, string>();
            if (foreignKeys.Any(fk => fk.TableName == "foUsers"))
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userMap[reader["ID"].ToString()] = reader["FullName"].ToString();
                        }
                    }
                }
            }

            foreach (var row in tableData)
            {
                foreach (var column in columns)
                {
                    if (row.ContainsKey(column))
                    {
                        var rawValue = row[column]?.ToString();

                        if (column.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrWhiteSpace(rawValue) && userMap.TryGetValue(rawValue, out var fullName))
                            {
                                row[$"{column}_Display"] = fullName;
                            }
                        }
                        else
                        {
                            var fk = foreignKeys.FirstOrDefault(f => f.ColumnName == column && f.TableName != "foUsers");
                            if (fk != null && fkOptions.ContainsKey(fk.TableName))
                            {
                                var match = fkOptions[fk.TableName].FirstOrDefault(opt => opt.Value == rawValue);
                                if (match != null)
                                {
                                    row[$"{column}_Display"] = match.Text;
                                }
                            }
                        }
                    }
                }
            }

            int pageSize = 20;
            var paginatedData = tableData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((double)tableData.Count / pageSize);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.searchText = searchText;
            ViewBag.readwriteaccess = "R";

            TempData["DataManagementBreadcrumbX"] = JsonConvert.SerializeObject(new DataManagementBreadcrumbX
            {
                PreviousScreen = "Index",
                Parameters = new Dictionary<string, string>
        {
            { "tablename", tablename },
            { "description", tableDescription.Replace("_", " ") },
            { "userid", userid.ToString() },
            { "pageNumber", pageNumber.ToString() }
        }
            });
            TempData.Keep("DataManagementBreadcrumbX");

            return View(new TableViewModel
            {
                UserId = userid,
                TableName = tablename,
                Columns = columns,
                TableData = paginatedData,
                PrimaryKeyColumn = GetPrimaryKeyColumn(tablename),
                ForeignKeys = foreignKeys
            });
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

        private List<foReportTable> GetReportTables(int userid, int reportsID)
        {
            var query = "SELECT * FROM dbo.foReportTable WHERE UserID = @UserID AND ParentID = 1 AND ReportsID = ";
            var userReportList = _dbHelper.ExecuteQuery<foReportTable>(query, new { UserID = userid });
            return userReportList;
        }
        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            var prefixes = _dbHelper.ExecuteQuery<foTablePrefix>(query);
            return prefixes;
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

        private string GetForeignKeyDescription(string tableName, object foreignKeyValue)
        {
            string description = string.Empty;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // 🟢 1. Handle foUsers directly
                if (tableName.Equals("foUsers", StringComparison.OrdinalIgnoreCase))    
                {
                    string userQuery = "SELECT FirstName + ' ' + LastName AS FullName FROM foUsers WHERE ID = @Id";
                    using (var cmd = new SqlCommand(userQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", foreignKeyValue);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            return result.ToString();
                    }
                }

                // 🔍 2. Check if it's a maintenance table (prefix match from foTablePrefixes)
                bool isMaintenance = false;
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM foTablePrefixes 
            WHERE @TableName LIKE Prefix + '%' AND Description = 'Maintenance'";

                using (var checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@TableName", tableName);
                    isMaintenance = (int)checkCmd.ExecuteScalar() > 0;
                }

                if (isMaintenance)
                {
                    // 🛠 3. Maintenance table: return [Description] directly
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
                    // 🧠 4. Composite fallback for other FK tables (non-maintenance)
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
                                string col = reader["COLUMN_NAME"].ToString();
                                string type = reader["DATA_TYPE"].ToString().ToLower();

                                // Wrap non-varchar in CONVERT
                                if (type != "varchar" && type != "nvarchar" && type != "text")
                                {
                                    col = $"CONVERT(varchar, {col})";
                                }

                                columns.Add(col);
                            }
                        }
                    }

                    if (columns.Count > 0)
                    {
                        string selectClause = string.Join(" + ' | ' + ", columns);
                        string compositeQuery = $"SELECT {selectClause} FROM {tableName} WHERE ID = @Id";

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


        private Dictionary<string, object> GetRecordById(string tablename, int id)
        {
            var record = new Dictionary<string, object>();
            string query = $"SELECT * FROM {tablename} WHERE ID = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                record[reader.GetName(i)] = reader[i];
                            }
                        }
                    }
                }
            }

            return record;
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


        private List<SelectListItem> GetForeignKeyDropdownData(string referencedTableName)
        {
            var dropdownData = new List<SelectListItem>();

            string query = $"SELECT ID, Description FROM {referencedTableName} WHERE Active = 1"; // Assuming 'Description' exists

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dropdownData.Add(new SelectListItem
                            {
                                Value = reader.GetInt64(0).ToString(),  // Use GetInt64 instead of GetInt32
                                Text = reader.GetString(1)
                            });
                        }
                    }
                }
            }

            return dropdownData;
        }

        private List<string> GetTableColumns(string tableName, int reportId, out bool hasQR)
        {
            hasQR = false;
            var columns = new List<string>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var columnQuery = new SqlCommand("SELECT ColumnQuery FROM foReportTable WHERE TableName = @TableName AND ReportsID = @ReportID", conn)
            {
                Parameters = { new SqlParameter("@TableName", tableName), new SqlParameter("@ReportID", reportId) }
            }.ExecuteScalar()?.ToString()?.Trim();

            var ignoredColumns = GetIgnoredColumns(conn).Select(c => c.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (columnQuery == "*")
            {
                // Expand *
                var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", conn);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var col = reader["COLUMN_NAME"].ToString();
                    if (!ignoredColumns.Contains(col) && !col.Equals("QR", StringComparison.OrdinalIgnoreCase))
                    {
                        columns.Add(col);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(columnQuery))
            {
                // Parse specific list, skip ignored, allow QR alias detection
                columns = columnQuery.Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !ignoredColumns.Contains(c) || c.Contains("'") || c.Contains(" as ", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (columnQuery.IndexOf("'QR' as QR", StringComparison.OrdinalIgnoreCase) >= 0)
                    hasQR = true;
            }

            return columns;
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
        private List<Dictionary<string, object>> GetTableData(string tableName, int reportId, bool hasQR)
        {
            var tableData = new List<Dictionary<string, object>>();
            string columnQuery;

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            columnQuery = new SqlCommand("SELECT ColumnQuery FROM foReportTable WHERE TableName = @TableName AND ReportsID = @ReportID", conn)
            {
                Parameters = { new SqlParameter("@TableName", tableName), new SqlParameter("@ReportID", reportId) }
            }.ExecuteScalar()?.ToString()?.Trim();

            var ignoredColumns = GetIgnoredColumns(conn).Select(c => c.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var columns = new List<string>();

            if (columnQuery == "*")
            {
                var cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", conn);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var col = reader["COLUMN_NAME"].ToString();
                    if (!ignoredColumns.Contains(col) && !col.Equals("QR", StringComparison.OrdinalIgnoreCase))
                    {
                        columns.Add(col);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(columnQuery))
            {
                columns = columnQuery.Split(',')
                    .Select(c => c.Trim())
                    .Where(c => !ignoredColumns.Contains(c) || c.Contains("'") || c.Contains(" as ", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (columns.Count == 0) return tableData;

            var selectQuery = $"SELECT {string.Join(", ", columns)} FROM {tableName} WHERE Active = 1";
            using var dataCmd = new SqlCommand(selectQuery, conn);
            using var dataReader = dataCmd.ExecuteReader();
            while (dataReader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    row[dataReader.GetName(i)] = dataReader[i];
                }
                tableData.Add(row);
            }

            return tableData;
        }



        private string GetPrimaryKeyColumn(string tablename)
        {
            string primaryKeyColumn = string.Empty;
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = @TableName";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);
                    primaryKeyColumn = command.ExecuteScalar()?.ToString();
                }
            }
            return primaryKeyColumn;
        }
        public IActionResult ExportToExcel(int userid, int reportid, string tablename)
        {
            // Step 1: Get columns and data
            bool hasQR;
            var columns = GetTableColumns(tablename, reportid, out hasQR);
            var data = GetTableData(tablename, reportid, hasQR);

            // Step 2: Get foreign key metadata
            var foreignKeys = GetForeignKeyColumns(tablename);

            // Step 3: Create Excel workbook
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Export");

            // Step 4: Header row (rename FK headers if needed)
            for (int i = 0; i < columns.Count; i++)
            {
                string colName = columns[i];
                var fk = foreignKeys.FirstOrDefault(f => f.ColumnName.Equals(colName, StringComparison.OrdinalIgnoreCase));

                if (fk != null)
                {
                    // Rename column from e.g. "AssettypeID" to "Assettype"
                    worksheet.Cell(1, i + 1).Value = fk.ColumnDescription;
                }
                else
                {
                    worksheet.Cell(1, i + 1).Value = colName;
                }
            }

            // Step 5: Data rows
            for (int row = 0; row < data.Count; row++)
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    string colName = columns[col];
                    object value = data[row].ContainsKey(colName) ? data[row][colName] : "";

                    // Replace FK ID with description
                    var fk = foreignKeys.FirstOrDefault(f => f.ColumnName.Equals(colName, StringComparison.OrdinalIgnoreCase));
                    if (fk != null && value != null && value != DBNull.Value)
                    {
                        value = GetForeignKeyDescription(fk.TableName, value);
                    }

                    worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
                }
            }

            // Step 6: Return Excel file with clean filename
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string cleanedName = CleanTableName(tablename);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{cleanedName}.xlsx");
        }

        public IActionResult ExportSingleToExcel(int reportid, int id, string tablename)
        {
            bool hasQR;
            var columns = GetTableColumns(tablename, reportid, out hasQR);
            var row = GetRecordById(tablename, id);
            var foreignKeys = GetForeignKeyColumns(tablename);

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Record");

            // Header and single row
            for (int i = 0; i < columns.Count; i++)
            {
                string colName = columns[i];

                // Column header: use FK description if applicable
                var fk = foreignKeys.FirstOrDefault(f => f.ColumnName.Equals(colName, StringComparison.OrdinalIgnoreCase));
                string header = fk != null ? fk.ColumnDescription : colName;
                worksheet.Cell(1, i + 1).Value = header;

                // Cell value: convert FK ID to description
                object value = row.ContainsKey(colName) ? row[colName] : "";
                if (fk != null && value != null && value != DBNull.Value)
                {
                    value = GetForeignKeyDescription(fk.TableName, value);
                }

                worksheet.Cell(2, i + 1).Value = value?.ToString() ?? "";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string cleanedName = CleanTableName(tablename);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{cleanedName}.xlsx");
        }

        private string CleanTableName(string rawName)
        {
            if (rawName.StartsWith("tbl_tran_", StringComparison.OrdinalIgnoreCase))
                return rawName.Replace("tbl_tran_", "").Replace("_", " ");
            if (rawName.StartsWith("tbl_md_", StringComparison.OrdinalIgnoreCase))
                return rawName.Replace("tbl_md_", "").Replace("_", " ");
            if (rawName.StartsWith("tbl_", StringComparison.OrdinalIgnoreCase))
                return rawName.Replace("tbl_", "").Replace("_", " ");
            return rawName.Replace("_", " ");
        }


    }
}
