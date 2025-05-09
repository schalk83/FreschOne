using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using System.Data;

namespace FreschOne.Controllers
{
    public class foTableController : BaseController
    {
        public foTableController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var tables = new List<foTable>();

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM foTable WHERE Active = 1", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(new foTable
                    {
                        ID = (long)reader["ID"],
                        SchemaName = reader["SchemaName"].ToString(),
                        TableName = reader["TableName"].ToString()
                    });
                }
            }

            // Get all columns grouped by TableID
            var columnMap = new Dictionary<long, List<foTableColumns>>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM foTableColumns where Active = 1", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var column = new foTableColumns
                    {
                        ID = (long)reader["ID"],
                        TableID = (long)reader["TableID"],
                        ColumnName = reader["ColumnName"].ToString(),
                        ColumnOrder = Convert.ToInt64(reader["ColumnOrder"]),
                        ColumnMaxLength = Convert.ToInt64(reader["ColumnMaxLength"]),
                        ColumnPrecision = Convert.ToInt64(reader["ColumnPrecision"]),
                        ColumnIsNullable = (bool)reader["ColumnIsNullable"]
                    };

                    if (!columnMap.ContainsKey(column.TableID))
                        columnMap[column.TableID] = new List<foTableColumns>();

                    columnMap[column.TableID].Add(column);
                }
            }
            ViewBag.ColumnMap = columnMap;


            return View(tables);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            return View(new foTable());
        }

        [HttpPost]
        public IActionResult Create(foTable table, int userid)
        {
            if (!ModelState.IsValid)
                return View(table);

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    INSERT INTO foTable (SchemaName, TableName, Active)
                    VALUES (@SchemaName, @TableName, 1)", conn);

                cmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTable table = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    table = new foTable
                    {
                        ID = (long)reader["ID"],
                        SchemaName = reader["SchemaName"].ToString(),
                        TableName = reader["TableName"].ToString()
                    };
                }
            }

            return View(table);
        }

        [HttpPost]
        public IActionResult Edit(foTable table, int userid)
        {
            if (!ModelState.IsValid)
                return View(table);

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    UPDATE foTable SET 
                        SchemaName = @SchemaName,
                        TableName = @TableName
                    WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@SchemaName", table.SchemaName);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@ID", table.ID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Check if table is referenced in TableColumns
                var cmd = new SqlCommand("SELECT COUNT(*) FROM TableColumns WHERE TableID = @TableID", conn);
                cmd.Parameters.AddWithValue("@TableID", id);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    TempData["ErrorMessage"] = $"❌ This table is linked to {count} column(s) and cannot be deleted. Remove columns first before trying again.";
                    return RedirectToAction("Index", new { userid });
                }

                // Soft delete
                var deleteCmd = new SqlCommand("UPDATE foTable SET Active = 0 WHERE ID = @ID", conn);
                deleteCmd.Parameters.AddWithValue("@ID", id);
                deleteCmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Table deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }
    }
}
