﻿using Microsoft.AspNetCore.Mvc;
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

            string ReportName = GetReportName(reportid);
            ViewBag.ReportName = ReportName;

            var columns = GetTableColumns(tablename,reportid);  // Get the columns for the table
            var tableData = GetTableData(tablename,reportid);   // Get the data for the table


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
            ViewBag.readwriteaccess = "R";
            ViewBag.ReportID = reportid;


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

        private List<string> GetTableColumns(string tableName, int reportId)
        {
            var columns = new List<string>();
            string columnQuery = string.Empty;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Step 1: Get columnQuery from foReportTable
                string query = "SELECT columnQuery FROM foReportTable WHERE TableName = @TableName AND ReportsID = @ReportID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.Parameters.AddWithValue("@ReportID", reportId);
                    columnQuery = command.ExecuteScalar()?.ToString();
                }

                // Step 2: Get ignored columns dynamically
                var ignoredColumns = GetIgnoredColumns(connection)
                    .Select(c => c.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase); // For fast exclusion

                // Step 3: Determine columns to return
                if (!string.IsNullOrEmpty(columnQuery))
                {
                    if (columnQuery.Trim() == "*")
                    {
                        string allColumnsQuery = @"
                    SELECT COLUMN_NAME 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName
                    ORDER BY ORDINAL_POSITION";

                        using (var columnCmd = new SqlCommand(allColumnsQuery, connection))
                        {
                            columnCmd.Parameters.AddWithValue("@TableName", tableName);
                            using var reader = columnCmd.ExecuteReader();
                            while (reader.Read())
                            {
                                var columnName = reader["COLUMN_NAME"].ToString();
                                if (!ignoredColumns.Contains(columnName))
                                {
                                    columns.Add(columnName);
                                }
                            }
                        }
                    }
                    else
                    {
                        // If a specific list is given, filter it too just in case
                        columns = columnQuery.Split(',')
                                             .Select(c => c.Trim())
                                             .Where(c => !string.IsNullOrEmpty(c) && !ignoredColumns.Contains(c))
                                             .ToList();
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
        private List<Dictionary<string, object>> GetTableData(string tableName, int reportId)
        {
            var tableData = new List<Dictionary<string, object>>();
            string columnQuery = string.Empty;

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Step 1: Retrieve columnQuery from foReportTable
                string query = "SELECT columnQuery FROM foReportTable WHERE TableName = @TableName AND ReportsID = @ReportID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);
                    command.Parameters.AddWithValue("@ReportID", reportId);
                    columnQuery = command.ExecuteScalar()?.ToString();
                }

                // Step 2: Get ignored columns from your custom table
                var ignoredColumns = GetIgnoredColumns(connection)
                    .Select(c => c.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase); // Fast lookup

                var columns = new List<string>();

                // Step 3: Decide how to get the list of columns
                if (!string.IsNullOrEmpty(columnQuery) && columnQuery.Trim() == "*")
                {
                    string allColumnsQuery = @"
                SELECT COLUMN_NAME 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = @TableName
                ORDER BY ORDINAL_POSITION";

                    using (var columnCmd = new SqlCommand(allColumnsQuery, connection))
                    {
                        columnCmd.Parameters.AddWithValue("@TableName", tableName);
                        using var reader = columnCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var columnName = reader["COLUMN_NAME"].ToString();
                            if (!ignoredColumns.Contains(columnName))
                            {
                                columns.Add(columnName);
                            }
                        }
                    }
                }
                else
                {
                    // Parse specific columns, and exclude ignored ones
                    columns = columnQuery.Split(',')
                                         .Select(c => c.Trim())
                                         .Where(c => !string.IsNullOrEmpty(c) && !ignoredColumns.Contains(c))
                                         .ToList();
                }

                // If no columns are valid, return empty result
                if (columns.Count == 0)
                {
                    return tableData;
                }

                // Step 4: Fetch data with the allowed columns
                string selectQuery = $"SELECT {string.Join(", ", columns)} FROM {tableName} WHERE Active = 1";

                using (var dataCmd = new SqlCommand(selectQuery, connection))
                {
                    using var reader = dataCmd.ExecuteReader();
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
