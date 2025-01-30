using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class TableXController : BaseController
    {
        public TableXController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        public IActionResult Index(int userid, string tablename, string readwriteaccess)
        {
            SetUserAccess(userid);

            ViewBag.userid = userid;

            // Get table data dynamically based on table name
            var columns = GetTableColumns(tablename);  // Get the columns for the table
            var tableData = GetTableData(tablename);   // Get the data for the table
            var primaryKeyColumn = GetPrimaryKeyColumn(tablename); // Get the primary key column for the table

            var viewModel = new TableViewModel
            {
                UserId = userid,
                TableName = tablename,
                Columns = columns,
                TableData = tableData,
                PrimaryKeyColumn = primaryKeyColumn
            };

            return View(viewModel);
        }

        public IActionResult Edit(int id, string tablename)
        {
            // Logic for editing a record based on PK
            // Retrieve the data and pass it to an Edit view
            return View();
        }

        public IActionResult Delete(int id, string tablename)
        {
            // Logic for deleting a record based on PK
            return RedirectToAction("Index");
        }

        public IActionResult Create(int userid, string tablename)
        {
            // Logic for creating a new record
            return View();
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

        private List<Dictionary<string, object>> GetTableData(string tablename)
        {
            var tableData = new List<Dictionary<string, object>>();
            string query = $"SELECT * FROM {tablename}";

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
