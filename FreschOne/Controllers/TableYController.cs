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
    public class TableYController : BaseController
    {
        public TableYController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, int PKID, string PKColumn, string tablename, int pageNumber = 1, string searchText = "", string returnURL = "")
        {
            // 🧭 Set breadcrumb tracking
            TempData["DataManagementBreadcrumbY"] = JsonConvert.SerializeObject(new DataManagementBreadcrumbY
            {
                PreviousScreen = "Index",
                Parameters = new Dictionary<string, string>
        {
            { "tablename", tablename },
            { "userid", userid.ToString() },
            { "PKID" , PKID.ToString() },
            { "PKColumn", PKColumn },
            { "pageNumber", pageNumber.ToString() }
        }
            });
            TempData.Keep("DataManagementBreadcrumbY");

            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }
            TempData.Keep("DataManagementBreadcrumbX");

            EnsureAuditFieldsExist(tablename);
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var columns = GetTableColumns(tablename);
            var tableData = GetTableData(PKID, PKColumn, tablename);

            // 🔍 Search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                tableData = tableData
                    .Where(row => row.Values.Any(v => v?.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();
            }

            // 🎯 DB-defined foreign keys
            var foreignKeys = GetForeignKeyColumns(tablename, PKColumn);

            // 💡 Add implicit foUserID_ columns
            var foUserColumns = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c))
                .Select(c => new ForeignKeyInfo
                {
                    ColumnName = c,
                    TableName = "foUsers",
                    ColumnDescription = "User"
                });

            foreignKeys.AddRange(foUserColumns);

            // 📦 FK options by TableName
            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                if (!fkOptions.ContainsKey(fk.TableName))
                {
                    fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                }
            }

            // 👤 Special: Resolve foUsers into FirstName + LastName
            var userLookup = new Dictionary<string, string>();
            if (foreignKeys.Any(fk => fk.TableName == "foUsers"))
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader["ID"].ToString();
                            string name = reader["FullName"].ToString();
                            userLookup[id] = name;
                        }
                    }
                }

                // Replace raw foUserID_* values with full names
                foreach (var row in tableData)
                {
                    foreach (var column in columns.Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase)))
                    {
                        var raw = row.ContainsKey(column) ? row[column]?.ToString() : null;
                        if (!string.IsNullOrWhiteSpace(raw) && userLookup.TryGetValue(raw, out var fullName))
                        {
                            row[$"{column}_Display"] = fullName;
                        }
                    }
                }
            }

            // 🧠 General FK display replacement (excluding foUsers)
            foreach (var row in tableData)
            {
                foreach (var fk in foreignKeys.Where(f => f.TableName != "foUsers"))
                {
                    if (row.ContainsKey(fk.ColumnName))
                    {
                        var rawValue = row[fk.ColumnName]?.ToString();
                        var match = fkOptions[fk.TableName].FirstOrDefault(o => o.Value == rawValue);
                        if (match != null)
                        {
                            row[$"{fk.ColumnName}_Display"] = match.Text;
                        }
                    }
                }
            }

            // 📄 Pagination
            int pageSize = 20;
            var paginatedData = tableData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((double)tableData.Count / pageSize);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.searchText = searchText;

            ViewBag.PKID = PKID;
            ViewBag.PKColumn = PKColumn;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;

            var tablePrefixes = GetTablePrefixes();
            var tableDescription = tablePrefixes
                .Where(p => tablename.StartsWith(p.Prefix))
                .Select(p => tablename.Replace(p.Prefix, ""))
                .FirstOrDefault() ?? tablename;
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            var viewModel = new TableViewModel
            {
                UserId = userid,
                TableName = tablename,
                Columns = columns,
                TableData = paginatedData,
                PrimaryKeyColumn = GetPrimaryKeyColumn(tablename),
                ForeignKeys = foreignKeys
            };

            return View(viewModel);
        }


        public IActionResult ForeignTables_Index(string tablename, int userid, bool isAjax = false)
        {
            try
            {
                // Retrieve the user's access data from foUserTable where Active = 1
                var userAccessList = GetUserTables(userid).Where(x => x.Active).ToList();

                // Get the list of tables the user has access to
                var userTables = userAccessList.Select(x => x.TableName).ToList();

                // Fetch related foreign key tables for the given tablename, filtered by user access
                var relatedTables = GetForeignTables(tablename, userTables);

                // Fetch the table prefixes
                var tablePrefixes = GetTablePrefixes();

                // Remove the prefix from the ChildTable names and dynamically assign the foreign key column
                foreach (var table in relatedTables)
                {
                    var childTableName = table["ChildTable"].ToString();

                    // Check if the ChildTable name starts with any prefix and remove it
                    foreach (var prefix in tablePrefixes)
                    {
                        if (childTableName.StartsWith(prefix.Prefix))
                        {
                            // Remove the prefix from the table name
                            table["ChildTable"] = childTableName;
                            table["ChildTableDescription"] = childTableName.Substring(prefix.Prefix.Length);


                            // Dynamically get the second column name as the foreign key
                            var foreignKeyColumn = GetTableColumns(childTableName).Skip(1).FirstOrDefault(); // Get the 2nd column
                            table["PKColumn"] = foreignKeyColumn;

                            break; // Stop once we find the matching prefix
                        }
                    }
                }

                if (isAjax)
                {
                    return Json(relatedTables);
                }

                return View(relatedTables);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        private List<Dictionary<string, object>> GetForeignTables(string tablename, List<string> userTables)
        {
            var result = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var query = @"
        SELECT tp.name AS ChildTable, cp.name AS ForeignKey
        FROM sys.foreign_keys AS fk
        INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
        INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
        INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
        INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
        INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
        WHERE tr.name = @TableName
        AND tp.name LIKE '%tbl_%' ORDER BY 1";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }

                    // Filter only those tables that the user has access to
                    if (userTables.Contains(row["ChildTable"].ToString()))
                    {
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        private List<foUserTable> GetUserTables(int userid)
        {
            var query = "SELECT * FROM dbo.foUserTable WHERE UserID = @UserID";
            var userAccessList = _dbHelper.ExecuteQuery<foUserTable>(query, new { UserID = userid });
            return userAccessList;
        }
        private List<foTablePrefix> GetTablePrefixes()
        {
            var query = "SELECT * FROM dbo.foTablePrefixes WHERE Active = 1";
            var prefixes = _dbHelper.ExecuteQuery<foTablePrefix>(query);
            return prefixes;
        }


        private List<ForeignKeyInfo> GetForeignKeyColumns(string tablename, string PKColumn)
        {
            var foreignKeys = new List<ForeignKeyInfo>();
            string query = @"
        SELECT 
            c.name AS ColumnName,
            ref_tab.name AS ReferencedTableName,
            REPLACE(
                CASE 
                    WHEN c.name LIKE 'foUserID_%' THEN REPLACE(c.name, 'foUserID_', '')
                    WHEN c.name LIKE '%ID' THEN LEFT(c.name, LEN(c.name) - 2)
                    ELSE c.name
                END, '_', ' ') AS ColumnDescription
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
            AND c.name != @PKColumn 
            AND c.name LIKE '%ID'";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);
                command.Parameters.AddWithValue("@PKColumn", PKColumn);

                using var reader = command.ExecuteReader();
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

            return foreignKeys;
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



        private Dictionary<string, object> GetRecordById(string tablename, int id)
        {
            var record = new Dictionary<string, object>();
            string query = $"SELECT * FROM {tablename} WHERE ID = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        record[reader.GetName(i)] = reader[i];
                    }
                }
            }

            return record;
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

        public IActionResult Edit(int id, int PKID, string PKColumn, string tablename, int userid, int pageNumber, string returnURL)
        {
            TempData.Keep("DataManagementBreadcrumbY");

            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }
            TempData.Keep("DataManagementBreadcrumbX");

            if (TempData["DataManagementBreadcrumbY"] != null)
            {
                ViewBag.DataManagementBreadcrumbY = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbY"].ToString());
            }
            TempData.Keep("DataManagementBreadcrumbY");

            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var record = GetRecordById(tablename, id);
            var columns = GetTableColumns(tablename);
            var foreignKeys = GetForeignKeyColumns(tablename, PKColumn);

            // Add implicit foUserID_ FK logic
            var foUserColumns = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c))
                .Select(c => new ForeignKeyInfo
                {
                    ColumnName = c,
                    TableName = "foUsers",
                    ColumnDescription = "User"
                });

            foreignKeys.AddRange(foUserColumns);

            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                if (!fkOptions.ContainsKey(fk.TableName))
                {
                    fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                }
            }

            // Special case: foUsers - preload full name lookup
            var userLookup = new Dictionary<string, string>();
            if (foreignKeys.Any(fk => fk.TableName == "foUsers"))
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var userId = reader["ID"].ToString();
                            var fullName = reader["FullName"].ToString();
                            userLookup[userId] = fullName;
                        }
                    }
                }

                foreach (var column in columns.Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase)))
                {
                    if (record.ContainsKey(column))
                    {
                        var userId = record[column]?.ToString();
                        if (!string.IsNullOrWhiteSpace(userId) && userLookup.TryGetValue(userId, out var fullName))
                        {
                            record[$"{column}_Display"] = fullName;
                        }
                    }
                }
            }

            // Replace other FK values with display text (excluding foUsers)
            foreach (var fk in foreignKeys.Where(f => f.TableName != "foUsers"))
            {
                if (record.ContainsKey(fk.ColumnName))
                {
                    var rawValue = record[fk.ColumnName]?.ToString();
                    var options = fkOptions[fk.TableName];
                    var match = options.FirstOrDefault(o => o.Value == rawValue);
                    record[$"{fk.ColumnName}_Display"] = match?.Text ?? "";
                }
            }

            var columnTypes = new Dictionary<string, string>();
            var columnLengths = new Dictionary<string, int>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"
        SELECT c.name AS ColumnName, t.name AS ColumnType, c.length
        FROM syscolumns c
        JOIN systypes t ON c.xusertype = t.xusertype
        WHERE c.id = OBJECT_ID(@TableName)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    columnTypes.Add(reader["ColumnName"].ToString(), reader["ColumnType"].ToString());
                    columnLengths.Add(reader["ColumnName"].ToString(), Convert.ToInt32(reader["length"]));
                }
            }

            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var foreignKey in foreignKeys)
            {
                foreignKeyOptions[foreignKey.ColumnName] = GetForeignKeyOptions(foreignKey.TableName);
            }

            var tablePrefixes = GetTablePrefixes();
            var tableDescription = "";
            foreach (var prefix in tablePrefixes)
            {
                if (tablename.Contains(prefix.Prefix))
                {
                    tableDescription = tablename.Replace(prefix.Prefix.ToString(), "");
                }
            }
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            ViewBag.id = id;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;
            ViewBag.PKID = PKID;
            ViewBag.PKColumn = PKColumn;
            ViewBag.pageNumber = pageNumber;

            // Fetch attachments
            string attachmentQuery = @"
        SELECT ID, AttachmentDescription, Attachment , DateAdded, ( select FirstName + ' ' + LastName from foUsers where ID = UserID ) AS UserAdded
        FROM foTableAttachments 
        WHERE tablename = @tablename AND PKID = @PKID";

            var attachments = new List<Dictionary<string, object>>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(attachmentQuery, connection);
                command.Parameters.AddWithValue("@tablename", tablename);
                command.Parameters.AddWithValue("@PKID", id);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    attachments.Add(new Dictionary<string, object>
            {
                { "ID", reader["ID"] },
                { "AttachmentDescription", reader["AttachmentDescription"] },
                { "Attachment", reader["Attachment"] },
                { "DateAdded", reader["DateAdded"] },
                { "UserAdded", reader["UserAdded"] }
            });
                }
            }

            ViewBag.Attachments = attachments;

            var viewModel = new TableEditViewModel
            {
                TableName = tablename,
                Columns = columns,
                Record = record,
                ForeignKeys = foreignKeys,
                ForeignKeyOptions = foreignKeyOptions
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Update(int id, int PKID, string PKColumn, string tablename, int userid, IFormCollection form, int pageNumber, string returnURL, string[] AttachmentDescriptions, List<IFormFile> Attachments)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues["ID"] = form["ID"].ToString();

            foreach (var key in form.Keys)
            {
                if (key == "ID") continue;

                string value = form[key].ToString().Trim();

                if (string.IsNullOrEmpty(value))
                {
                    updatedValues[key] = DBNull.Value;
                }
                else if (key.StartsWith("attachment_"))
                {
                    string[] parts = value.Split(';');
                    string newDescription = parts.Length > 0 ? parts[0].Trim() : "";
                    string existingFilePath = (parts.Length > 1 ? parts[1].Trim() : "").Split(',')[0];

                    var file = form.Files["file_" + key];
                    string finalFilePath = existingFilePath;

                    if (file != null && file.Length > 0)
                    {
                        var root = Directory.GetCurrentDirectory();
                        var folder = Path.Combine(root, "Attachments", tablename);
                        Directory.CreateDirectory(folder);

                        var newFileName = Path.GetFileName(file.FileName);
                        finalFilePath = Path.Combine("Attachments", tablename, newFileName);

                        using var stream = new FileStream(Path.Combine(root, finalFilePath), FileMode.Create);
                        file.CopyTo(stream);
                    }

                    updatedValues[key] = string.IsNullOrWhiteSpace(newDescription) && string.IsNullOrWhiteSpace(finalFilePath)
                        ? DBNull.Value
                        : $"{newDescription};{finalFilePath}";
                }
                else if (key.ToLower().Contains("is") || key.ToLower().Contains("active") || key.ToLower().EndsWith("flag"))
                {
                    updatedValues[key] = (value.ToLower() == "true" || value == "1") ? 1 : 0;
                }
                else if (DateTime.TryParse(value, out var parsedDate))
                {
                    updatedValues[key] = parsedDate;
                }
                else if (int.TryParse(value, out var parsedInt))
                {
                    updatedValues[key] = parsedInt;
                }
                else
                {
                    updatedValues[key] = value;
                }
            }

            updatedValues["ModifiedUserID"] = userid;
            var setClauses = updatedValues.Where(kv => kv.Key != "ID")
                                          .Select(kv => $"{kv.Key} = @{kv.Key}")
                                          .ToList();
            setClauses.Add("ModifiedDate = GETDATE()");

            var query = $"UPDATE {tablename} SET {string.Join(", ", setClauses)} WHERE ID = @Id";

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", updatedValues["ID"]);

                    foreach (var kv in updatedValues.Where(kv => kv.Key != "ID"))
                    {
                        var param = cmd.Parameters.Add($"@{kv.Key}", SqlDbType.NVarChar);
                        param.Value = kv.Value ?? DBNull.Value;
                    }

                    cmd.ExecuteNonQuery();

                    // ✅ Save new attachments (Add Attachments section)
                    if (Attachments != null && Attachments.Count > 0)
                    {
                        var root = Directory.GetCurrentDirectory();
                        var folder = Path.Combine(root, "Attachments");
                        Directory.CreateDirectory(folder);

                        for (int i = 0; i < Attachments.Count; i++)
                        {
                            var file = Attachments[i];
                            var description = (AttachmentDescriptions.Length > i) ? AttachmentDescriptions[i] : "No Description";

                            if (file.Length > 0)
                            {
                                var path = Path.Combine(folder, Guid.NewGuid() + Path.GetExtension(file.FileName));
                                using var stream = new FileStream(path, FileMode.Create);
                                file.CopyTo(stream);

                                var insert = @"
                            INSERT INTO foTableAttachments (tablename, PKID, AttachmentDescription, Attachment, UserID, DateAdded)
                            VALUES (@tablename, @PKID, @desc, @path, @userid, @date)";

                                using var insertCmd = new SqlCommand(insert, conn);
                                insertCmd.Parameters.AddWithValue("@tablename", tablename);
                                insertCmd.Parameters.AddWithValue("@PKID", id);
                                insertCmd.Parameters.AddWithValue("@desc", description);
                                insertCmd.Parameters.AddWithValue("@path", path);
                                insertCmd.Parameters.AddWithValue("@userid", userid);
                                insertCmd.Parameters.AddWithValue("@date", DateTime.Now);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", new { userid, PKID, PKColumn, tablename, pageNumber });
        }


        [HttpPost]
        public IActionResult Delete(int id, int userid, int PKID, string PKColumn, string tablename)
        {
            // Construct the SQL query to update the record
            var setClauses = new List<string>
    {
        "Active = 0",  // Set Active to false
        "DeletedUserID = @UserID",  // Set the DeletedUserID
        "DeletedDate = GETDATE()"  // Set the DeletedDate to current date
    };

            var query = $"UPDATE {tablename} SET {string.Join(", ", setClauses)} WHERE ID = @Id";

            // Execute the SQL query
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    // Add parameters to prevent SQL injection
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UserID", userid);

                    // Execute the query
                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Optionally, add a message or log here if needed
                        TempData["Message"] = "Record deactivated successfully.";
                    }
                    else
                    {
                        // Handle the case where no rows were updated (e.g., record not found)
                        TempData["ErrorMessage"] = "Record not found or already deactivated.";
                    }
                }
            }

            // Redirect or return a result based on your needs
            return RedirectToAction("Index", new { userid, PKID, PKColumn, tablename });

        }

        public IActionResult Create(int userid, int PKID, string PKColumn, string tablename, string returnURL)
        {

            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }

            TempData.Keep("DataManagementBreadcrumbX");

            if (TempData["DataManagementBreadcrumbY"] != null)
            {
                ViewBag.DataManagementBreadcrumbY = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbY"].ToString());
            }

            TempData.Keep("DataManagementBreadcrumbY");


            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            // Retrieve columns for the table
            var columns = GetTableColumns(tablename);

            // Retrieve foreign key columns for the table
            var foreignKeys = GetForeignKeyColumns(tablename, PKColumn);

            // Get column types and lengths from systypes and syscolumns
            var columnTypes = new Dictionary<string, string>(); // Dictionary to hold column types
            var columnLengths = new Dictionary<string, int>(); // Dictionary to hold column lengths

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"
            SELECT c.name AS ColumnName, t.name AS ColumnType, c.length
            FROM syscolumns c
            JOIN systypes t ON c.xusertype = t.xusertype
            WHERE c.id = OBJECT_ID(@TableName)";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);

                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    columnTypes.Add(reader["ColumnName"].ToString(), reader["ColumnType"].ToString());
                    columnLengths.Add(reader["ColumnName"].ToString(), Convert.ToInt32(reader["length"]));
                }
            }

            // Create a dictionary to hold the foreign key options
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();

            foreach (var foreignKey in foreignKeys)
            {
                foreignKeyOptions[foreignKey.ColumnName] = GetForeignKeyOptions(foreignKey.TableName);
            }

            var tablePrefixes = GetTablePrefixes();
            var tableDescription = "";
            // Check if the ChildTable name starts with any prefix and remove it
            foreach (var prefix in tablePrefixes)
            {
                if (tablename.Contains(prefix.Prefix))
                {
                    tableDescription = tablename.Replace(prefix.Prefix.ToString(), "");
                }
            }
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            // Prepare the ViewBag for additional parameters
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.PKColumn = PKColumn;
            ViewBag.PKID = PKID;
            ViewBag.ColumnTypes = columnTypes; // Pass column types to the view
            ViewBag.ColumnLengths = columnLengths; // Pass column lengths to the view

            // Create the view model for TableCreateViewModel
            var viewModel = new TableCreateViewModel
            {
                TableName = tablename,
                Columns = columns,
                ForeignKeys = foreignKeys,
                ForeignKeyOptions = foreignKeyOptions,
                ColumnTypes = columnTypes,
                ColumnLengths = columnLengths,
                Record = new Dictionary<string, object>()  // Initialize an empty dictionary for form values
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(int userid, int PKID, string PKColumn, string tablename, Dictionary<string, string> formData, string[] AttachmentDescriptions, List<IFormFile> Attachments, string returnURL)
        {
            //SetUserAccess(userid);

            // Prepare the data for insertion into the table
            var columns = GetTableColumns(tablename);
            var tableColumns = new List<string>();
            var values = new List<object>();

            // Prepare placeholders and parameters for the insert query
            var valuePlaceholders = new List<string>();
            var parameters = new Dictionary<string, object>();

            foreach (var column in columns)
            {
                if (formData.ContainsKey(column))
                {
                    tableColumns.Add(column);
                    values.Add(formData[column]);

                    // Use column name as parameter name
                    string parameterName = $"@{column}";
                    valuePlaceholders.Add(parameterName);
                    parameters.Add(parameterName, formData[column]);
                }
            }


            // Add standard columns
            tableColumns.Add("Active");
            valuePlaceholders.Add("@Active");
            parameters["@Active"] = "1";

            // Add standard columns
            tableColumns.Add("CreatedUserID");
            valuePlaceholders.Add("@CreatedUserID");
            parameters["@CreatedUserID"] = userid;
            tableColumns.Add("CreatedDate");
            valuePlaceholders.Add("GETDATE()");

            // Construct the SQL insert statement dynamically
            var columnNames = string.Join(", ", tableColumns);
            var valuePlaceholdersString = string.Join(", ", valuePlaceholders);

            string query = $"INSERT INTO {tablename} ({columnNames}) OUTPUT INSERTED.ID VALUES ({valuePlaceholdersString})"; // Output the newly created ID

            long newPKID;

            // Execute the insert query and get the newly generated ID
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                // Add parameters to the SQL command
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                }

                // Execute query and retrieve the newly generated ID
                newPKID = (long)command.ExecuteScalar();
            }

            // If attachments exist, insert them into foTableAttachments
            if (Attachments != null && Attachments.Any())
            {
                var rootPath = Directory.GetCurrentDirectory();
                var attachmentsFolder = Path.Combine(rootPath, "Attachments");

                // Ensure the directory exists
                if (!Directory.Exists(attachmentsFolder))
                {
                    Directory.CreateDirectory(attachmentsFolder);
                }

                for (int i = 0; i < Attachments.Count; i++)
                {
                    var attachment = Attachments[i];
                    var description = (AttachmentDescriptions != null && AttachmentDescriptions.Length > i) ? AttachmentDescriptions[i] : "No Description";

                    if (attachment.Length > 0)
                    {
                        var filePath = Path.Combine(attachmentsFolder, Guid.NewGuid() + Path.GetExtension(attachment.FileName));

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            attachment.CopyTo(stream);
                        }

                        // Insert attachment details into foTableAttachments
                        string attachmentQuery = @"
                INSERT INTO foTableAttachments (tablename, PKID, AttachmentDescription, Attachment, UserID, DateAdded) 
                VALUES (@tablename, @PKID, @AttachmentDescription, @Attachment, @userid, @Dateadded)";

                        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                        connection.Open();
                        using var command = new SqlCommand(attachmentQuery, connection);
                        command.Parameters.AddWithValue("@tablename", tablename);
                        command.Parameters.AddWithValue("@PKID", newPKID);
                        command.Parameters.AddWithValue("@AttachmentDescription", description);
                        command.Parameters.AddWithValue("@Attachment", filePath);
                        command.Parameters.AddWithValue("@userid", userid);
                        command.Parameters.AddWithValue("@Dateadded", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }

            //return RedirectToAction("Index", new { userid = userid, tablename = tablename });
            return RedirectToAction("Index", new { userid, PKID, PKColumn, tablename });
        }

        private List<SelectListItem> GetForeignKeyDropdownData(string referencedTableName)
        {
            var dropdownData = new List<SelectListItem>();

            string query = $"SELECT ID, Description FROM {referencedTableName} WHERE Active = 1"; // Assuming 'Description' exists

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    dropdownData.Add(new SelectListItem
                    {
                        Value = reader.GetInt64(0).ToString(),  // Use GetInt64 instead of GetInt32
                        Text = reader.GetString(1)
                    });
                }
            }

            return dropdownData;
        }

        private List<string> GetTableColumns(string tablename)
        {
            var columns = new List<string>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // 🔹 Step 1: Get ignored column names from foTableColumnsToIgnore
                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                // 🔹 Step 2: Get all columns from INFORMATION_SCHEMA.COLUMNS
                string query = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string columnName = reader["COLUMN_NAME"].ToString();
                    if (!ignoredSet.Contains(columnName))
                    {
                        columns.Add(columnName);
                    }
                }
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


        private List<Dictionary<string, object>> GetTableData(int PKID, string PKColumn, string tablename)
        {
            var tableData = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // 🔹 Get ignored columns from foTableColumnsToIgnore
                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                // 🔹 Get all column names from the table, excluding the ignored ones
                var columns = new List<string>();
                string columnQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";

                using (var columnCmd = new SqlCommand(columnQuery, connection))
                {
                    columnCmd.Parameters.AddWithValue("@tableName", tablename);
                    using var reader = columnCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        if (!ignoredSet.Contains(columnName))
                        {
                            columns.Add(columnName);
                        }
                    }
                }

                if (columns.Count == 0)
                    return tableData; // No columns to select

                // 🔹 Build final SELECT statement
                string selectQuery = $@"
            SELECT {string.Join(", ", columns)} 
            FROM {tablename} 
            WHERE {PKColumn} = @PKID AND Active = 1";

                using (var command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@PKID", PKID);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }
                            tableData.Add(row);
                        }
                    }
                }
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
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);
                primaryKeyColumn = command.ExecuteScalar()?.ToString();
            }
            return primaryKeyColumn;
        }

        [HttpPost]
        public IActionResult AddAttachments(int id, int PKID, string PKColumn, string tablename, int userid, string[] AttachmentDescriptions, List<IFormFile> Attachments)
        {
            if (Attachments == null || Attachments.Count == 0)
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return RedirectToAction("Edit", new { id, PKID, PKColumn, tablename, userid });
            }

            var rootPath = Directory.GetCurrentDirectory();
            var attachmentsFolder = Path.Combine(rootPath, "Attachments\\" + tablename);

            // Ensure the directory exists
            if (!Directory.Exists(attachmentsFolder))
            {
                Directory.CreateDirectory(attachmentsFolder);
            }

            for (int i = 0; i < Attachments.Count; i++)
            {
                var attachment = Attachments[i];
                var description = AttachmentDescriptions.Length > i ? AttachmentDescriptions[i] : "No Description";

                if (attachment.Length > 0)
                {
                    var filePath = Path.Combine(attachmentsFolder, attachment.FileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        attachment.CopyTo(stream);
                    }

                    // Insert attachment details into foTableAttachments
                    string query = @"
                INSERT INTO foTableAttachments (tablename, PKID, AttachmentDescription, Attachment, UserID, DateAdded) 
                VALUES (@tablename, @PKID, @AttachmentDescription, @Attachment, @userid, @Dateadded)";

                    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                    connection.Open();
                    using var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@tablename", tablename);
                    command.Parameters.AddWithValue("@PKID", id);
                    command.Parameters.AddWithValue("@AttachmentDescription", description);
                    command.Parameters.AddWithValue("@Attachment", filePath);
                    command.Parameters.AddWithValue("@userid", userid);
                    command.Parameters.AddWithValue("@Dateadded", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Edit", new { id, PKID, PKColumn, tablename, userid });
        }

        [HttpPost]
        public IActionResult DeleteAttachment(int attachmentId, string tablename, long PKID, int userid)
        {
            string query = "DELETE FROM foTableAttachments WHERE ID = @ID";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", attachmentId);
                command.ExecuteNonQuery();
            }

            return RedirectToAction("Edit", new { id = PKID, tablename, userid });
        }

        public IActionResult SelectParent(string tablename, int userid, int pageNumber = 1, string searchText = "")
        {
            SetUserAccess(userid);

            var mapping = GetForeignKeyParentMapping(tablename); // e.g., FKColumn = "StudentID"
            var parentTable = mapping.ParentTable;

            ViewBag.userid = userid;
            ViewBag.ChildTable = tablename;
            ViewBag.ParentTable = parentTable;
            ViewBag.FKColumn = mapping.FKColumn;

            var fullData = GetParentTableData(parentTable);

            // 🔎 Apply search
            if (!string.IsNullOrEmpty(searchText))
            {
                fullData = fullData
                    .Where(row => row.Values.Any(v => v != null && v.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // 🔗 Get columns and foreign keys
            var columns = GetTableColumns(parentTable);
            var foreignKeys = GetForeignKeyColumns(parentTable);

            // 🧠 Inject foUserID_* logic
            var foUserColumns = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c))
                .Select(c => new ForeignKeyInfo { ColumnName = c, TableName = "foUsers", ColumnDescription = "User" });

            foreignKeys.AddRange(foUserColumns);

            // 🧠 Build FK options
            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                if (!fkOptions.ContainsKey(fk.TableName))
                    fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
            }

            // 🧠 foUsers bulk lookup
            var userLookup = new Dictionary<string, string>();
            if (foreignKeys.Any(fk => fk.TableName == "foUsers"))
            {
                using var conn = GetConnection();
                conn.Open();
                using var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userLookup[reader["ID"].ToString()] = reader["FullName"].ToString();
                }
            }

            // 🔁 Replace values
            foreach (var row in fullData)
            {
                foreach (var fk in foreignKeys)
                {
                    var rawValue = row.ContainsKey(fk.ColumnName) ? row[fk.ColumnName]?.ToString() : null;

                    if (fk.TableName == "foUsers" && userLookup.ContainsKey(rawValue))
                    {
                        row[$"{fk.ColumnName}_Display"] = userLookup[rawValue];
                    }
                    else if (fk.TableName != "foUsers" && fkOptions.TryGetValue(fk.TableName, out var options))
                    {
                        var match = options.FirstOrDefault(o => o.Value == rawValue);
                        if (match != null)
                            row[$"{fk.ColumnName}_Display"] = match.Text;
                    }
                }
            }

            // 📄 Pagination
            int pageSize = 20;
            var pagedData = fullData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((double)fullData.Count / pageSize);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.searchText = searchText;

            return View("SelectParent", new TableViewModel
            {
                TableName = parentTable,
                UserId = userid,
                Columns = columns,
                TableData = pagedData,
                ForeignKeys = foreignKeys
            });
        }


        public class ForeignKeyParentMapping
        {
            public string ParentTable { get; set; } = "";
            public string FKColumn { get; set; } = "";
        }

        private ForeignKeyParentMapping GetForeignKeyParentMapping(string childTable)
        {
            var mapping = new ForeignKeyParentMapping();

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();

            var query = @"
        SELECT TOP 1
            tr.name AS ParentTable,
            cp.name AS FKColumn
        FROM sys.foreign_keys AS fk
        INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
        INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
        INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
        INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
        WHERE tp.name = @ChildTable";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChildTable", childTable);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                mapping.ParentTable = reader["ParentTable"].ToString() ?? "";
                mapping.FKColumn = reader["FKColumn"].ToString() ?? "";
            }
            else
            {
                throw new Exception($"❌ Could not determine parent table for '{childTable}'. Check if it has a foreign key.");
            }

            return mapping;
        }

        private List<Dictionary<string, object>> GetParentTableData(string tablename)
        {
            var tableData = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                var columns = new List<string>();
                string columnQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";

                using (var columnCmd = new SqlCommand(columnQuery, connection))
                {
                    columnCmd.Parameters.AddWithValue("@tableName", tablename);
                    using var reader = columnCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        if (!ignoredSet.Contains(columnName))
                        {
                            columns.Add(columnName);
                        }
                    }
                }

                if (columns.Count == 0)
                    return tableData;

                string selectQuery = $@"
            SELECT {string.Join(", ", columns)} 
            FROM {tablename} 
            WHERE Active = 1";

                using (var command = new SqlCommand(selectQuery, connection))
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader[i];
                        }
                        tableData.Add(row);
                    }
                }
            }

            return tableData;
        }



    }
}