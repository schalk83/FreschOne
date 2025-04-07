using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Controllers
{
    public class foTableColumnsToIgnoreController : BaseController
    {
        public foTableColumnsToIgnoreController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var list = new List<foTableColumnsToIgnore>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, ColumnName FROM foTableColumnsToIgnore", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new foTableColumnsToIgnore
                    {
                        ID = (long)reader["ID"],
                        ColumnName = reader["ColumnName"]?.ToString()
                    });
                }
            }

            return View(list);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            return View();
        }

        [HttpPost]
        public IActionResult Create(foTableColumnsToIgnore model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foTableColumnsToIgnore (ColumnName) VALUES (@ColumnName)", conn);
                cmd.Parameters.AddWithValue("@ColumnName", model.ColumnName ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTableColumnsToIgnore model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foTableColumnsToIgnore WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foTableColumnsToIgnore
                    {
                        ID = (long)reader["ID"],
                        ColumnName = reader["ColumnName"]?.ToString()
                    };
                }
            }



            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foTableColumnsToIgnore model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foTableColumnsToIgnore SET ColumnName = @ColumnName WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@ColumnName", model.ColumnName ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        [HttpPost]
        public IActionResult Delete(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foTableColumnsToIgnore WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Column ignore entry deleted.";
            return RedirectToAction("Index", new { userid });
        }

        private List<SelectListItem> GetValidFKColumns(string tableName)
        {
            var list = new List<SelectListItem>();
            var ignoredColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ID" };

            using (var conn = GetConnection())
            {
                // Get ignored column names
                var ignoreCmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore", conn);
                conn.Open();
                using (var reader = ignoreCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ignoredColumns.Add(reader["ColumnName"].ToString());
                    }
                }

                // Get all column names for the specified table
                var columnCmd = new SqlCommand(@"
            SELECT c.name AS ColumnName
            FROM sys.columns c
            JOIN sys.tables t ON c.object_id = t.object_id
            WHERE t.name = @TableName", conn);

                columnCmd.Parameters.AddWithValue("@TableName", tableName);
                using (var reader = columnCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var col = reader["ColumnName"].ToString();
                        if (!ignoredColumns.Contains(col))
                        {
                            list.Add(new SelectListItem { Value = col, Text = col });
                        }
                    }
                }
            }

            return list;
        }

    }
}
