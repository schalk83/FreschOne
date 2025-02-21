using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            AND c.name LIKE '%ID'";

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

        public IActionResult Edit(int id, string tablename, int userid, int pageNumber)
        {
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


        [HttpPost]
        public IActionResult Update(int id, int userid, string tablename, IFormCollection form, int pageNumber)
        {
            // Create a dictionary to store the updated values
            var updatedValues = new Dictionary<string, object>();

            // Add the ID manually
            updatedValues["ID"] = form["ID"].ToString();  // Convert StringValues to string

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

            tableColumns.Add("CreateUserID");
            valuePlaceholders.Add("@CreateUserID");
            parameters["@CreateUserID"] = userid;
            tableColumns.Add("CreateDate");
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
                using (var command = new SqlCommand(query, connection))
                {
                    // Add parameters to the SQL command
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                    }

                    // Execute query and retrieve the newly generated ID
                    newPKID = (long)command.ExecuteScalar();
                }
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

                        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                        {
                            connection.Open();
                            using (var command = new SqlCommand(attachmentQuery, connection))
                            {
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
            string query = $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName 
                                    AND COLUMN_NAME NOT IN('Active', 'CreateUserID', 'CreateDate', 
                                    'ModifiedUserID', 'ModifiedDate', 'DeletedUserID', 'DeletedDate')";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TableName", tablename);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    columns.Add(reader.GetString(0));
                }            
            }
            return columns;
        }

        private List<Dictionary<string, object>> GetTableData(string tableName)
        {
            var tableData = new List<Dictionary<string, object>>();
            var excludedColumns = new HashSet<string>
    {
        "Active", "CreateUserID", "CreateDate",
        "ModifiedUserID", "ModifiedDate", "DeletedUserID", "DeletedDate"
    };

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Get only required column names (excluding the audit fields)
                string columnQuery = $@"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @tableName 
            AND COLUMN_NAME NOT IN ('Active', 'CreateUserID', 'CreateDate', 
                                    'ModifiedUserID', 'ModifiedDate', 'DeletedUserID', 'DeletedDate')";

                var columns = new List<string>();
                using (var columnCmd = new SqlCommand(columnQuery, connection))
                {
                    columnCmd.Parameters.AddWithValue("@tableName", tableName);
                    using var reader = columnCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        columns.Add(reader["COLUMN_NAME"].ToString());
                    }
                }

                if (columns.Count == 0)
                {
                    return tableData; // No data to fetch
                }

                // Construct the final SELECT query with only the required columns
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
