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

        public IActionResult Index(int userid, string tablename, int pageNumber = 1, string searchText = "")
        {

            EnsureAuditFieldsExist(tablename);
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var columns = GetTableColumns(tablename);  // Get the columns for the table
            var tableData = GetTableData(tablename);   // Get the data for the table


            // Apply column-based filtering if any column's search text is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                tableData = tableData.Where(row =>
                    row.Values.Any(value => value.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Fetch all foreign key columns and replace them with corresponding descriptions
            var foreignKeys = GetForeignKeyColumns(tablename);
            foreach (var row in tableData)
            {
                foreach (var foreignKey in foreignKeys)
                {
                    if (row.ContainsKey(foreignKey.ColumnName))
                    {
                        var foreignKeyValue = row[foreignKey.ColumnName];
                        if (foreignKeyValue != DBNull.Value)
                        {
                            row[foreignKey.ColumnName] = GetForeignKeyDescription(foreignKey.TableName, foreignKeyValue);
                        }
                    }
                }
            }

            // Pagination logic
            int pageSize = 20;
            var paginatedData = tableData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)tableData.Count / pageSize);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.searchText = searchText;

            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
                         
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
            ViewBag.tableDescription = tableDescription.Replace("_"," ");

            // Set the breadcrumb for tracking
            TempData["DataManagementBreadcrumbX"] = JsonConvert.SerializeObject(new DataManagementBreadcrumbX
            {
                PreviousScreen = "Index",
                Parameters = new Dictionary<string, string>
                {
                    { "tablename", tablename },
                    { "description",tableDescription.Replace("_"," ")},
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

        public IActionResult Edit(int id, string tablename, int userid, int pageNumber)
        {
            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }

            TempData.Keep("DataManagementBreadcrumbX");

            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            // Retrieve the record for the specified id
            var record = GetRecordById(tablename, id);

            // Get columns and their types for the table
            var columns = GetTableColumns(tablename);
            var foreignKeys = GetForeignKeyColumns(tablename);

            var relatedTables = GetRelatedTables(tablename, userid);
            ViewBag.RelatedTables = relatedTables;

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
            foreach (var prefix in tablePrefixes)
            {
                if (tablename.Contains(prefix.Prefix))
                {
                    tableDescription = tablename.Replace(prefix.Prefix.ToString(), "");
                }
            }
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            // Prepare the ViewBag for additional parameters
            ViewBag.id = id;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ColumnTypes = columnTypes;
            ViewBag.ColumnLengths = columnLengths;
            ViewBag.pageNumber = pageNumber;

            // Fetch existing attachments for this record
            string attachmentQuery = @"
        SELECT ID, AttachmentDescription, Attachment , DateAdded, ( select FirstName + ' ' + LastName from foUsers where ID = UserID ) AS UserAdded
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

            ViewBag.Attachments = attachments; // Pass attachments to the view

            // Create the view model
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


        public IActionResult Create(int userid, string tablename, string readwriteaccess) 
        {
            if (TempData["DataManagementBreadcrumbX"] != null)
            {
                ViewBag.DataManagementBreadcrumbX = JsonConvert.DeserializeObject<DataManagementBreadcrumbX>(TempData["DataManagementBreadcrumbX"].ToString());
            }

            TempData.Keep("DataManagementBreadcrumbX");

            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            // Retrieve columns for the table
            var columns = GetTableColumns(tablename);

            // Retrieve foreign key columns for the table
            var foreignKeys = GetForeignKeyColumns(tablename);

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



    }
}
