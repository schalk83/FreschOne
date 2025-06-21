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
    public class TableXController : BaseController
    {
        public TableXController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        public IActionResult Index(int userid, string tablename, int pageNumber = 1, string searchText = "")
        {
            EnsureAuditFieldsExist(tablename);
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var columns = GetTableColumns(tablename);
            var tableData = GetTableData(tablename);

            if (!string.IsNullOrEmpty(searchText))
            {
                tableData = tableData
                    .Where(row => row.Values.Any(v => v?.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) == true))
                    .ToList();
            }

            // 🎯 Normal FKs from DB
            var foreignKeys = GetForeignKeyColumns(tablename);

            // 🔍 Add implicit foUserID_ columns (resolved to foUsers)
            var foUserColumns = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c)) // Avoid duplicates
                .Select(c => new ForeignKeyInfo
                {
                    ColumnName = c,
                    TableName = "foUsers",
                    ColumnDescription = "User"
                });

            foreignKeys.AddRange(foUserColumns);

            // 📦 Fetch all FK options (grouped by TableName)
            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                if (!fkOptions.ContainsKey(fk.TableName))
                {
                    fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                }
            }

            // 🧠 Special bulk lookup for foUsers: ID -> "FirstName LastName"
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

                // Overwrite foUserID_ columns with full names directly
                foreach (var row in tableData)
                {
                    foreach (var column in columns.Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase)))
                    {
                        var rawValue = row.ContainsKey(column) ? row[column]?.ToString() : null;
                        if (!string.IsNullOrWhiteSpace(rawValue) && userLookup.TryGetValue(rawValue, out var fullName))
                        {
                            row[$"{column}_Display"] = fullName;
                        }
                    }
                }
            }

            // 🧠 Generic FK value replacement (skip foUsers, already done)
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
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;

            var tablePrefixes = GetTablePrefixes();
            var tableDescription = tablePrefixes
                .Where(p => tablename.StartsWith(p.Prefix))
                .Select(p => tablename.Replace(p.Prefix, ""))
                .FirstOrDefault() ?? tablename;
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

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

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);

                    using (var reader = command.ExecuteReader())
                    {
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

        private List<Dictionary<string, object>> GetRelatedTables(string tablename, int userid)
        {
            var userAccessList = GetUserTables(userid).Where(x => x.Active).ToList();
            var userTables = userAccessList.Select(x => x.TableName).ToList();
            var relatedTables = GetForeignTables(tablename, userTables);
            var tablePrefixes = GetTablePrefixes();

            foreach (var table in relatedTables)
            {
                var childTableName = table["ChildTable"].ToString();
                foreach (var prefix in tablePrefixes)
                {
                    if (childTableName.StartsWith(prefix.Prefix))
                    {
                        table["ChildTable"] = childTableName;
                        table["ChildTableDescription"] = childTableName.Substring(prefix.Prefix.Length);
                        table["PKColumn"] = GetTableColumns(childTableName).Skip(1).FirstOrDefault();
                        break;
                    }
                }
            }

            return relatedTables;
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

        public IActionResult Edit(int id, string tablename, int userid, int pageNumber)
        {
            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }
            TempData.Keep("DataManagementBreadcrumbX");

            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);
            var record = GetRecordById(tablename, id);

            var columns = GetTableColumns(tablename);
            var foreignKeys = GetForeignKeyColumns(tablename);

            // 🔍 Add implicit foUserID_ columns to foreignKeys (if not already there)
            var foUserColumns = columns
                .Where(c => c.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                .Where(c => !foreignKeys.Any(fk => fk.ColumnName == c)) // Avoid duplication
                .Select(c => new ForeignKeyInfo
                {
                    ColumnName = c,
                    TableName = "foUsers",
                    ColumnDescription = "User"
                });

            foreignKeys.AddRange(foUserColumns);

            // 🔥 Get FK options per FK column
            var fkOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                fkOptions[fk.ColumnName] = GetForeignKeyOptions(fk.TableName);
            }

            // 🧠 Resolve FK values to display values (excluding foUsers for now)
            foreach (var fk in foreignKeys.Where(fk => fk.TableName != "foUsers"))
            {
                if (record.ContainsKey(fk.ColumnName))
                {
                    var rawValue = record[fk.ColumnName]?.ToString();
                    var match = fkOptions[fk.ColumnName].FirstOrDefault(o => o.Value == rawValue);
                    record[$"{fk.ColumnName}_Display"] = match?.Text ?? "";
                }
            }

            // 🧠 Bulk lookup of foUsers (for all foUserID_ fields)
            var userLookup = new Dictionary<string, string>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var uid = reader["ID"].ToString();
                        var name = reader["FullName"].ToString();
                        userLookup[uid] = name;
                    }
                }
            }

            // ✨ Resolve foUserID_ fields
            foreach (var key in record.Keys.Where(k => k.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                var userIdValue = record[key]?.ToString();
                if (!string.IsNullOrEmpty(userIdValue) && userLookup.TryGetValue(userIdValue, out var userFullName))
                {
                    record[$"{key}_Display"] = userFullName;
                }
            }

            // 🔗 Related tables
            var relatedTables = GetRelatedTables(tablename, userid);
            ViewBag.RelatedTables = relatedTables;

            // 🧬 Column types + lengths
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
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnTypes[reader["ColumnName"].ToString()] = reader["ColumnType"].ToString();
                        columnLengths[reader["ColumnName"].ToString()] = Convert.ToInt32(reader["length"]);
                    }
                }
            }

            // 🧩 FK dropdowns to ViewBag
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var foreignKey in foreignKeys)
            {
                foreignKeyOptions[foreignKey.ColumnName] = GetForeignKeyOptions(foreignKey.TableName);
            }

            // 📛 Table Description
            var tablePrefixes = GetTablePrefixes();
            var tableDescription = tablePrefixes
                .Where(p => tablename.StartsWith(p.Prefix))
                .Select(p => tablename.Replace(p.Prefix, ""))
                .FirstOrDefault() ?? tablename;

            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            // 📎 ViewBag
            ViewBag.id = id;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;
            ViewBag.pageNumber = pageNumber;

            // 📎 Attachments
            string attachmentQuery = @"
        SELECT ID, AttachmentDescription, Attachment, DateAdded,
               (SELECT FirstName + ' ' + LastName FROM foUsers WHERE ID = UserID) AS UserAdded
        FROM foTableAttachments 
        WHERE tablename = @tablename AND PKID = @PKID";

            var attachments = new List<Dictionary<string, object>>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(attachmentQuery, connection))
                {
                    command.Parameters.AddWithValue("@tablename", tablename);
                    command.Parameters.AddWithValue("@PKID", id);

                    using (var reader = command.ExecuteReader())
                    {
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
                }
            }

            ViewBag.Attachments = attachments;

            // 🎁 Final ViewModel
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
        public IActionResult Update(int id, int userid, string tablename, IFormCollection form, int pageNumber)
        {


            // Create a dictionary to store the updated values
            var updatedValues = new Dictionary<string, object>();

            // Add the ID manually
            updatedValues["ID"] = form["ID"].ToString();  // Convert StringValues to string

            string[] attachmentDescriptions = form["AttachmentDescriptions"];
            var uploadedFiles = form.Files;

            // Loop through the form collection to get all keys and values
            foreach (var key in form.Keys)
            {
                if (key == "ID") continue; // Skip the ID field, which is handled separately

                string value = form[key].ToString().Trim(); // Trim to avoid whitespace issues

                // Handle empty values as NULL
                if (string.IsNullOrEmpty(value))
                {
                    updatedValues[key] = DBNull.Value;
                }
                //attachments
                else if (key.StartsWith("attachment_"))
                {
                    // ✅ Extract the existing values from the database entry
                    string[] parts = value.Split(';');
                    string newDescription = parts.Length > 0 ? parts[0].Trim() : ""; // Extract description
                    string existingFilePath = (parts.Length > 1 ? parts[1].Trim() : "").Split(',')[0]; // Extract ONLY the file path

                    Console.WriteLine($"📌 Processing Attachment: Key={key}, New Desc='{newDescription}', Existing File='{existingFilePath}'");

                    string finalFilePath = existingFilePath; // Default to the existing file path unless a new file is uploaded

                    // ✅ Fetch new file from form input
                    var file = form.Files["file_" + key];

                    if (file != null && file.Length > 0)
                    {
                        var rootPath = Directory.GetCurrentDirectory();
                        var uploadsFolder = Path.Combine(rootPath, "Attachments", tablename);

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string newFileName = Path.GetFileName(file.FileName);
                        finalFilePath = Path.Combine("Attachments", tablename, newFileName);

                        using (var stream = new FileStream(Path.Combine(rootPath, finalFilePath), FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        Console.WriteLine($"✅ File '{newFileName}' uploaded successfully to '{finalFilePath}'");
                    }

                    // ✅ Store NULL if both description & file path are empty
                    if (string.IsNullOrWhiteSpace(newDescription) && string.IsNullOrWhiteSpace(finalFilePath))
                    {
                        updatedValues[key] = DBNull.Value;
                    }
                    else
                    {
                        updatedValues[key] = $"{newDescription};{finalFilePath}";
                    }

                    // 🚀 REMOVE any `file_attachment_*` keys to prevent SQL errors
                    string fileKey = "file_" + key;
                    if (updatedValues.ContainsKey(fileKey))
                    {
                        updatedValues.Remove(fileKey);
                        Console.WriteLine($"🗑️ Removed invalid key from SQL query: {fileKey}");
                    }
                }

                else if (key.ToLower().Contains("is") || key.ToLower().Contains("active") || key.ToLower().EndsWith("flag"))
                {
                    // Convert to bit (Boolean) - 1 for true, 0 for false
                    updatedValues[key] = (value.ToLower() == "true" || value == "1") ? 1 : 0;
                }
                else if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    // Convert to DateTime if valid
                    updatedValues[key] = parsedDate;
                }
                else if (int.TryParse(value, out int parsedInt))
                {
                    // Convert to int if valid
                    updatedValues[key] = parsedInt;
                }
                else
                {
                    // Default to string
                    updatedValues[key] = value;
                }
            }

            // Log the dictionary to verify the data
            foreach (var kv in updatedValues)
            {
                Console.WriteLine($"Key: {kv.Key}, Value: {(kv.Value == DBNull.Value ? "NULL" : kv.Value)}");
            }

            // Add standard columns for tracking modifications
            updatedValues["ModifiedUserID"] = userid;

            // Use SQL GETDATE() directly (no quotes)
            string modifiedDateField = "ModifiedDate = GETDATE()";

            // If you need to update the database, build the update query here
            var setClauses = updatedValues
                .Where(kv => kv.Key != "ID") // Skip the ID column
                .Select(kv => $"{kv.Key} = @{kv.Key}")
                .ToList();

            setClauses.Add(modifiedDateField); // Add GETDATE() field directly

            var query = $"UPDATE {tablename} SET {string.Join(", ", setClauses)} WHERE ID = @Id";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    // Add the ID parameter
                    command.Parameters.AddWithValue("@Id", updatedValues["ID"]);

                    // Add parameters for each updated value
                    foreach (var kv in updatedValues)
                    {
                        if (kv.Key != "ID")
                        {
                            var parameter = command.Parameters.Add($"@{kv.Key}", SqlDbType.NVarChar); // Default to string

                            // Assign NULL if the value is DBNull.Value
                            if (kv.Value == DBNull.Value)
                            {
                                parameter.Value = DBNull.Value;
                                parameter.IsNullable = true;
                            }
                            else
                            {
                                parameter.Value = kv.Value;
                            }
                        }
                    }

                    command.ExecuteNonQuery(); // Execute the query

                    if (uploadedFiles != null && uploadedFiles.Count > 0)
                    {
                        for (int i = 0; i < uploadedFiles.Count; i++)
                        {
                            var file = uploadedFiles[i];
                            var description = (attachmentDescriptions.Length > i) ? attachmentDescriptions[i] : "No Description";

                            if (file.Length > 0)
                            {
                                var folder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
                                Directory.CreateDirectory(folder);

                                var savePath = Path.Combine(folder, Guid.NewGuid() + Path.GetExtension(file.FileName));
                                using (var stream = new FileStream(savePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }

                                string insertAttach = @"
                        INSERT INTO foTableAttachments (tablename, PKID, AttachmentDescription, Attachment, UserID, DateAdded)
                        VALUES (@tablename, @PKID, @desc, @path, @userid, @date)";

                                using var attachCmd = new SqlCommand(insertAttach, connection);
                                attachCmd.Parameters.AddWithValue("@tablename", tablename);
                                attachCmd.Parameters.AddWithValue("@PKID", id);
                                attachCmd.Parameters.AddWithValue("@desc", description);
                                attachCmd.Parameters.AddWithValue("@path", savePath);
                                attachCmd.Parameters.AddWithValue("@userid", userid);
                                attachCmd.Parameters.AddWithValue("@date", DateTime.Now);

                                attachCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index", new { userid, tablename, pageNumber });
        }


        public IActionResult Create(int userid, string tablename, string readwriteaccess)
        {
            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }

            TempData.Keep("DataManagementBreadcrumbX");

            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var columns = GetTableColumns(tablename);
            var foreignKeys = GetForeignKeyColumns(tablename);

            // ✅ Add foUserID_ columns to foreignKeys if not explicitly defined
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

            // 🧬 Get column types and lengths
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
                    columnTypes[reader["ColumnName"].ToString()] = reader["ColumnType"].ToString();
                    columnLengths[reader["ColumnName"].ToString()] = Convert.ToInt32(reader["length"]);
                }
            }

            // 🎯 Foreign Key Options for dropdowns
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var fk in foreignKeys)
            {
                foreignKeyOptions[fk.ColumnName] = GetForeignKeyOptions(fk.TableName);
            }

            // 📛 Table description
            var tablePrefixes = GetTablePrefixes();
            var tableDescription = tablePrefixes
                .Where(p => tablename.StartsWith(p.Prefix))
                .Select(p => tablename.Replace(p.Prefix, ""))
                .FirstOrDefault() ?? tablename;

            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            // 🧠 ViewBags
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;

            var viewModel = new TableCreateViewModel
            {
                TableName = tablename,
                Columns = columns,
                ForeignKeys = foreignKeys,
                ForeignKeyOptions = foreignKeyOptions,
                ColumnTypes = columnTypes,
                ColumnLengths = columnLengths,
                Record = new Dictionary<string, object>() // empty for create
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(int userid, string tablename, Dictionary<string, string> formData, string[] AttachmentDescriptions, List<IFormFile> Attachments)
        {
            long newPKID;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var columns = GetTableColumns(tablename);
                var ignoredColumns = GetIgnoredColumns(connection);

                var tableColumns = new List<string>();
                var valuePlaceholders = new List<string>();
                var parameters = new Dictionary<string, object>();

                foreach (var column in columns)
                {
                    if (formData.ContainsKey(column))
                    {
                        tableColumns.Add(column);
                        string paramName = $"@{column}";
                        valuePlaceholders.Add(paramName);
                        parameters[paramName] = formData[column];
                    }
                }


                tableColumns.Add("Active");
                valuePlaceholders.Add("@Active");
                parameters["@Active"] = 1;
                tableColumns.Add("CreatedUserID");
                valuePlaceholders.Add("@CreatedUserID");
                parameters["@CreatedUserID"] = userid;
                tableColumns.Add("CreatedDate");
                valuePlaceholders.Add("GETDATE()");

                string columnNames = string.Join(", ", tableColumns);
                string valueClause = string.Join(", ", valuePlaceholders);
                string insertSql = $"INSERT INTO {tablename} ({columnNames}) OUTPUT INSERTED.ID VALUES ({valueClause})";

                using var cmd = new SqlCommand(insertSql, connection);
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }

                newPKID = (long)cmd.ExecuteScalar();
            }

            // ✅ Handle attachments
            if (Attachments != null && Attachments.Any())
            {
                var rootPath = Directory.GetCurrentDirectory();
                var attachmentsFolder = Path.Combine(rootPath, "Attachments");
                Directory.CreateDirectory(attachmentsFolder);

                for (int i = 0; i < Attachments.Count; i++)
                {
                    var attachment = Attachments[i];
                    var description = (AttachmentDescriptions != null && AttachmentDescriptions.Length > i) ? AttachmentDescriptions[i] : "No Description";

                    if (attachment.Length > 0)
                    {
                        var filePath = Path.Combine(attachmentsFolder, Guid.NewGuid() + Path.GetExtension(attachment.FileName));
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            attachment.CopyTo(stream);
                        }

                        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                        connection.Open();
                        string attachmentSql = @"
                    INSERT INTO foTableAttachments (tablename, PKID, AttachmentDescription, Attachment, UserID, DateAdded) 
                    VALUES (@tablename, @PKID, @AttachmentDescription, @Attachment, @userid, @DateAdded)";
                        using var cmd = new SqlCommand(attachmentSql, connection);
                        cmd.Parameters.AddWithValue("@tablename", tablename);
                        cmd.Parameters.AddWithValue("@PKID", newPKID);
                        cmd.Parameters.AddWithValue("@AttachmentDescription", description);
                        cmd.Parameters.AddWithValue("@Attachment", filePath);
                        cmd.Parameters.AddWithValue("@userid", userid);
                        cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction("Index", new { userid, tablename });
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

        private List<string> GetIgnoredColumns()
        {
            var ignoredColumns = new List<string>();
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using var cmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ignoredColumns.Add(reader["ColumnName"].ToString());
                }
            }
        return ignoredColumns;
        }


        private List<Dictionary<string, object>> GetTableData(string tableName)
        {
            var tableData = new List<Dictionary<string, object>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // 🔹 Get ignored columns from foTableColumnsToIgnore
                var ignoredColumns = GetIgnoredColumns(connection);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                // 🔹 Get all column names for the specified table
                string columnQuery = @"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @tableName";

                var columns = new List<string>();
                using (var columnCmd = new SqlCommand(columnQuery, connection))
                {
                    columnCmd.Parameters.AddWithValue("@tableName", tableName);
                    using var reader = columnCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string column = reader["COLUMN_NAME"].ToString();
                        if (!ignoredSet.Contains(column))
                        {
                            columns.Add(column);
                        }
                    }
                }

                if (columns.Count == 0)
                {
                    return tableData; // No data to fetch
                }

                // 🔹 Build SELECT query using remaining columns
                string query = $"SELECT {string.Join(", ", columns)} FROM {tableName} WHERE Active = 1";

                using (var command = new SqlCommand(query, connection))
                {
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
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);
                    primaryKeyColumn = command.ExecuteScalar()?.ToString();
                }
            }
            return primaryKeyColumn;
        }

        [HttpPost]
        public IActionResult AddAttachments(string tablename, long PKID, int userid, string[] AttachmentDescriptions, List<IFormFile> Attachments)
        {
            if (Attachments == null || Attachments.Count == 0)
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return RedirectToAction("Edit", new { id = PKID, tablename = tablename });
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

                    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                    {
                        connection.Open();
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@tablename", tablename);
                            command.Parameters.AddWithValue("@PKID", PKID);
                            command.Parameters.AddWithValue("@AttachmentDescription", description);
                            command.Parameters.AddWithValue("@Attachment", filePath);
                            command.Parameters.AddWithValue("@userid", userid);
                            command.Parameters.AddWithValue("@Dateadded", DateTime.Now);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

            return RedirectToAction("Edit", new { id = PKID, tablename = tablename, userid = userid });
        }

        [HttpPost]
        public IActionResult Delete(int id, string tablename, int userid)
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
            return RedirectToAction("Index", new { userid, tablename });
        }


        [HttpPost]
        public IActionResult DeleteAttachment(int attachmentId, string tablename, long PKID, int userid)
        {
            string query = "DELETE FROM foTableAttachments WHERE ID = @ID";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", attachmentId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Edit", new { id = PKID, tablename = tablename, userid = userid });
        }

        [HttpGet]
        public IActionResult GetSearchOptions2(string tableName, string columnName)
        {
            var result = new List<Dictionary<string, object>>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    var ignoredColumns = GetIgnoredColumns(conn);
                    var fkColumns = GetForeignKeyColumns(tableName).ToList();
                    var allColumns = GetTableColumns(tableName);

                    var needsUserLookup = false;

                    // 🧠 Determine if we need foUsers lookup
                    if (tableName == "foUsers")
                        needsUserLookup = true; 


                    // 🔁 Prepare FK options (including foUsers)
                    var fkOptions = new Dictionary<string, List<SelectListItem>>();
                    foreach (var fk in fkColumns)
                    {
                        if (!fkOptions.ContainsKey(fk.TableName))
                        {
                            fkOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                        }
                    }

                    // 👤 Bulk-load foUsers if needed
                    var userLookup = new Dictionary<string, string>();
                    if (needsUserLookup)
                    {
                        using (var userCmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn))
                        using (var reader = userCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var uid = reader["ID"].ToString();
                                var fullName = reader["FullName"].ToString();
                                userLookup[uid] = fullName;
                            }
                        }
                    }

                    // 🔄 Now read actual rows from the target table
                    var cmd = new SqlCommand($"SELECT * FROM {tableName} WHERE Active = 1", conn);
                    using var dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        row["ID"] = dataReader["ID"].ToString();

                        // ✅ Special case for foUsers
                        if (tableName == "foUsers")
                        {
                            var firstName = dataReader["FirstName"]?.ToString();
                            var lastName = dataReader["LastName"]?.ToString();
                            row["Display"] = $"{firstName} {lastName}".Trim();
                            result.Add(row);
                            continue;
                        }

                        var displayParts = new List<string>();

                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            var name = dataReader.GetName(i);
                            if (ignoredColumns.Contains(name) || name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                                continue;

                            var value = dataReader[name]?.ToString();
                            if (string.IsNullOrWhiteSpace(value)) continue;

                            var fkMatch = fkColumns.FirstOrDefault(x => x.ColumnName == name);
                            if (fkMatch != null)
                            {
                                if (fkMatch.TableName == "foUsers" && userLookup.TryGetValue(value, out var fullName))
                                {
                                    displayParts.Add(fullName);
                                }
                                else
                                {
                                    var displayValue = fkOptions[fkMatch.TableName]
                                        .FirstOrDefault(x => x.Value == value)?.Text ?? value;
                                    displayParts.Add(displayValue);
                                }
                            }
                            else
                            {
                                displayParts.Add(value);
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

        [HttpGet]
        public IActionResult GetSearchOptions(string tableName, string columnName)
        {
            var columns = new HashSet<string>();
            var rows = new List<Dictionary<string, object>>();

            try
            {
                using var conn = GetConnection();
                conn.Open();

                var ignoredColumns = GetIgnoredColumns(conn);
                var fkColumns = GetForeignKeyColumns(tableName).ToList();
                var allColumns = GetTableColumns(tableName);

                var needsUserLookup = tableName == "foUsers";
                var userLookup = new Dictionary<string, string>();

                if (needsUserLookup)
                {
                    using var userCmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn);
                    using var reader = userCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["ID"].ToString();
                        var fullName = reader["FullName"].ToString();
                        userLookup[id] = fullName;
                    }
                }

                var cmd = new SqlCommand($"SELECT * FROM {tableName} WHERE Active = 1", conn);
                using var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    var row = new Dictionary<string, object>();
                    var displayParts = new List<string>();

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        var name = dataReader.GetName(i);
                        if (ignoredColumns.Contains(name)) continue;

                        var value = dataReader[name]?.ToString();
                        if (string.IsNullOrWhiteSpace(value)) continue;

                        columns.Add(name); // collect column names

                        // foreign key override
                        var fkMatch = fkColumns.FirstOrDefault(fk => fk.ColumnName == name);
                        if (fkMatch != null)
                        {
                            if (fkMatch.TableName == "foUsers" && userLookup.TryGetValue(value, out var fullName))
                            {
                                displayParts.Add(fullName);
                                row[name] = fullName;
                            }
                            else
                            {
                                var options = GetForeignKeyOptions(fkMatch.TableName);
                                var resolved = options.FirstOrDefault(x => x.Value == value)?.Text ?? value;
                                displayParts.Add(resolved);
                                row[name] = resolved;
                            }
                        }
                        else
                        {
                            row[name] = value;

                            // ✅ Include core user fields if foUsers
                            if (needsUserLookup && (name == "FirstName" || name == "LastName" ))
                                displayParts.Add(value);
                            else if (!needsUserLookup)
                                displayParts.Add(value);
                        }
                    }

                    row["ID"] = dataReader["ID"].ToString();
                    if (!row.ContainsKey("Display") || string.IsNullOrWhiteSpace(row["Display"]?.ToString()))
                        row["Display"] = string.Join(" | ", displayParts);

                    rows.Add(row);
                }

                return Json(new
                {
                    Columns = columns.ToList(),
                    Rows = rows
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


    }
}
