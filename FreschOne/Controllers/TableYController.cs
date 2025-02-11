using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Controllers
{
    public class TableYController : BaseController
    {
        public TableYController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Index(int userid, int PKID, string PKColumn, string tablename, string readwriteaccess, int pageNumber = 1, string searchText = "")
        {
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            var columns = GetTableColumns(tablename);  // Get the columns for the table
            var tableData = GetTableData(PKID, PKColumn, tablename);   // Get the data for the table

            // Apply column-based filtering if any column's search text is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                tableData = tableData.Where(row =>
                    row.Values.Any(value => value.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Fetch all foreign key columns and replace them with corresponding descriptions
            var foreignKeys = GetForeignKeyColumns(tablename, PKColumn);
            foreach (var row in tableData)
            {
                foreach (var foreignKey in foreignKeys)
                {
                    if (foreignKey.ColumnName != PKColumn)
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
            }

            // Pagination logic
            int pageSize = 20;
            var paginatedData = tableData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)tableData.Count / pageSize);

            ViewBag.pageNumber = pageNumber;
            ViewBag.totalPages = totalPages;
            ViewBag.searchText = searchText;

            ViewBag.PKID = PKID;
            ViewBag.PKColumn = PKColumn;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            
            var tablePrefixes = GetTablePrefixes();
            var tableDescription = "";
            // Check if the ChildTable name starts with any prefix and remove it
            foreach (var prefix in tablePrefixes)
            {
                tableDescription = tablename.Substring(prefix.Prefix.Length);
            }
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


        private List<ForeignKeyInfo> GetForeignKeyColumns(string tablename, string PKColumn)
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
            parent_tab.name = @TableName and c.name != @PKColumn
            AND c.name LIKE '%ID'";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);
                    command.Parameters.AddWithValue("@PKColumn", PKColumn);

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

        public IActionResult Edit(int id, int PKID, string PKColumn, string tablename, int userid, string readwriteaccess)
        {
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);

            // Retrieve the record for the specified id
            var record = GetRecordById(tablename, id);

            // Get columns and their types for the table
            var columns = GetTableColumns(tablename);
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
                tableDescription = tablename.Substring(prefix.Prefix.Length);
            }
            ViewBag.tableDescription = tableDescription.Replace("_", " ");

            // Prepare the ViewBag for additional parameters
            ViewBag.id = id;
            ViewBag.userid = userid;
            ViewBag.tablename = tablename;
            ViewBag.ColumnTypes = columnTypes; // Pass column types to the view
            ViewBag.ColumnLengths = columnLengths; // Pass column lengths to the view

            ViewBag.PKID = PKID;
            ViewBag.PKColumn = PKColumn;

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
        public IActionResult Update(int id, int userid, int PKID, string PKColumn, string tablename, string readwriteaccess, IFormCollection form)
        {
            // Create a dictionary to store the updated values
            var updatedValues = new Dictionary<string, object>();

            // Add the ID manually
            updatedValues["ID"] = form["ID"].ToString();  // Convert StringValues to string

            // Loop through the form collection to get all keys and values
            foreach (var key in form.Keys)
            {
                // Skip the ID field, which is handled above
                if (key == "ID") continue;

                // Convert the value from StringValues to string (or appropriate type)
                updatedValues[key] = form[key].ToString();  // Convert StringValues to string
            }

            // Log the dictionary to verify the data
            foreach (var kv in updatedValues)
            {
                Console.WriteLine($"Key: {kv.Key}, Value: {kv.Value}");
            }

            // If you need to update the database, build the update query here
            var setClauses = updatedValues
                .Where(kv => kv.Key != "ID")  // Skip the ID column
                .Select(kv => $"{kv.Key} = @{kv.Key}")
                .ToList();

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
                            // Ensure the value is not a StringValues object
                            command.Parameters.AddWithValue($"@{kv.Key}", kv.Value ?? DBNull.Value);
                        }
                    }

                    command.ExecuteNonQuery(); // Execute the query
                }
            }

            return RedirectToAction("Index", new { userid = userid, PKID = PKID, PKColumn = PKColumn, tablename = tablename, readwriteaccess = readwriteaccess });
        }


        public IActionResult Delete(int id, string tablename)
        {
            // Logic for deleting a record based on PK
            return RedirectToAction("Index");
        }

        public IActionResult Create(int userid, int PKID, string PKColumn, string tablename, string readwriteaccess) 
        {
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
                tableDescription = tablename.Substring(prefix.Prefix.Length);
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
        public IActionResult Create(int userid, int PKID, string PKColumn, string tablename, string readwriteaccess, Dictionary<string, string> formData)
        {
            SetUserAccess(userid);
            GetUserReadWriteAccess(userid, tablename);


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

            // Construct the SQL insert statement dynamically
            var columnNames = string.Join(", ", tableColumns);
            var valuePlaceholdersString = string.Join(", ", valuePlaceholders);

            var query = $"INSERT INTO {tablename} ({columnNames}) VALUES ({valuePlaceholdersString})";

            // Execute the insert query
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

                    // Execute the insert query
                    command.ExecuteNonQuery();
                }
            }

//            return RedirectToAction("Index", new { userid = userid, tablename = tablename, readwriteaccess = readwriteaccess });
            return RedirectToAction("Index", new { userid = userid, PKID = PKID, PKColumn = PKColumn, tablename = tablename, readwriteaccess = readwriteaccess });

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
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";

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
                            columns.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return columns;
        }

        private List<Dictionary<string, object>> GetTableData(int PKID, string PKColumn, string tablename)
        {
            var tableData = new List<Dictionary<string, object>>();
            string query = $"SELECT * FROM {tablename} where {PKColumn} = {PKID} ";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
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
    }
}
