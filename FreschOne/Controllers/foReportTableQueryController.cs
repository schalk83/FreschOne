
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FreschOne.Models;
using System.Formats.Tar;
using System.Text.Json;

namespace FreschOne.Controllers
{
    public class foReportTableQueryController : BaseController
    {
        public foReportTableQueryController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(long? reportid, int userid)
        {

            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ReportID = reportid;
            ViewBag.ReportsDropdown = GetReportsSelectList();

            var list = new List<foReportTableQuery>();

            if (reportid == null)
                return View(list);
                
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                SELECT * FROM foReportTableQuery 
                WHERE ReportsID = @ReportsID AND Active = 1", conn);
                cmd.Parameters.AddWithValue("@ReportsID", reportid);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new foReportTableQuery
                    {
                        ID = (long)reader["ID"],
                        ReportsID = (long)reader["ReportsID"],
                        Query = reader["Query"]?.ToString(),
                        FormType = reader["FormType"]?.ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        TableDescription = reader["TableDescription"]?.ToString(),
                        Active = reader["Active"] as bool?
                    });
                }
            }

            return View(list);
        }

        public IActionResult Create(long reportid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.FormTypes = GetFormTypeSelectList();
            ViewBag.ReportID = reportid;
            
            ViewBag.TablePrefixes = GetTablePrefixes(); // For the radio buttons
            ViewBag.ValidTables = GetPrefixedTableNames();

            //return View(new foReportTableQuery { ReportsID = reportid });
            return View(new foReportTableQuery
            {
                ReportsID = reportid,
            });
        }


        [HttpPost]
        public IActionResult Create(foReportTableQuery model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
                       
            if (!IsQueryValid(model.Query))
            {
                ModelState.AddModelError("Query", "❌ Invalid SQL query. Please check your table name or column list.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FormTypes = GetFormTypeSelectList();
                ViewBag.ReportID = model.ReportsID;
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
        INSERT INTO foReportTableQuery 
        (ReportsID, Query, FormType, ColumnCount, TableDescription, Active)
        VALUES 
        (@ReportsID, @Query, @FormType, @ColumnCount, @TableDescription, 1)", conn);

                cmd.Parameters.AddWithValue("@ReportsID", model.ReportsID);
                cmd.Parameters.AddWithValue("@Query", (object?)model.Query ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", (object?)model.FormType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", (object?)model.ColumnCount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TableDescription", (object?)model.TableDescription ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { reportid = model.ReportsID, userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foReportTableQuery model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foReportTableQuery WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foReportTableQuery
                    {
                        ID = (long)reader["ID"],
                        ReportsID = (long)reader["ReportsID"],
                        Query = reader["Query"]?.ToString(),
                        FormType = reader["FormType"]?.ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        TableDescription = reader["TableDescription"]?.ToString(),
                        Active = reader["Active"] as bool?
                    };
                }
            }

            // Set ReportID for use in view
            ViewBag.ReportID = model?.ReportsID ?? 0;
            ViewBag.FormTypes = GetFormTypeSelectList();

            ViewBag.ValidTables = GetPrefixedTableNames();
            ViewBag.TablePrefixes = GetTablePrefixes(); // For the radio buttons

            return View(model);
        }


        [HttpPost]
        public IActionResult Edit(foReportTableQuery model, int userid)
        {
            
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!IsQueryValid(model.Query))
            {
                ModelState.AddModelError("Query", "❌ Invalid SQL query. Please check your table name or column list.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.FormTypes = GetFormTypeSelectList();
                ViewBag.ReportID = model.ReportsID;
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
        UPDATE foReportTableQuery 
        SET Query = @Query, FormType = @FormType, ColumnCount = @ColumnCount,
            TableDescription = @TableDescription
        WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@Query", (object?)model.Query ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", (object?)model.FormType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", (object?)model.ColumnCount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TableDescription", (object?)model.TableDescription ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { reportid = model.ReportsID, userid });
        }


        [HttpPost]
        public IActionResult Delete(long id, long reportid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foReportTableQuery WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Table entry deleted.";
            return RedirectToAction("Index", new { reportid, userid });
        }

        private List<SelectListItem> GetReportsSelectList()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, ReportName FROM foReports WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["ReportName"].ToString()
                    });
                }
            }
            return list;
        }

    

        [HttpGet]
        public JsonResult GetFKColumns(string tableName)
        {
            var columns = new List<string>();
            var ignoreList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = GetConnection())
            {
                conn.Open();

                // Step 1: Get ignored columns
                var ignoreCmd = new SqlCommand("SELECT ColumnName FROM foTableColumnsToIgnore", conn);
                using (var reader = ignoreCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ignoreList.Add(reader["ColumnName"].ToString());
                    }
                }

                // Step 2: Get table columns
                var columnCmd = new SqlCommand(@"
            SELECT COLUMN_NAME 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @TableName", conn);

                columnCmd.Parameters.AddWithValue("@TableName", tableName);

                using (var reader = columnCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var col = reader["COLUMN_NAME"].ToString();
                        if (!string.Equals(col, "ID", StringComparison.OrdinalIgnoreCase) && !ignoreList.Contains(col))
                        {
                            columns.Add(col);
                        }
                    }
                }
            }

            return Json(columns);
        }

        private bool IsQueryValid(string Query)
        {
            using var conn = GetConnection();
            var query = $"{Query}";

            try
            {
                using var cmd = new SqlCommand(query, conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public JsonResult TestQuery([FromBody] JsonElement data)
        {
            try
            {
                string Query = data.GetProperty("Query").GetString();

                using var conn = GetConnection();
                var cmd = new SqlCommand($"{Query}", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private List<SelectListItem> GetFormTypeSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "F", Text = "F" },
                new SelectListItem { Value = "T", Text = "T" }
            };
        }

        private List<SelectListItem> GetPrefixedTableNames()
        {
            var list = new List<SelectListItem>();

            using var conn = GetConnection();
            conn.Open();

            // Step 1: Get active prefixes
            var prefixes = new List<string>();
            using (var cmd = new SqlCommand("SELECT Prefix FROM foTablePrefixes WHERE Active = 1", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                    prefixes.Add(reader["Prefix"].ToString());
            }

            // Step 2: Build WHERE clause and get matching table names
            if (prefixes.Any())
            {
                var conditions = string.Join(" OR ", prefixes.Select((p, i) => $"name LIKE @p{i}"));
                var tableCmd = new SqlCommand($"SELECT name FROM sys.tables WHERE {conditions} ORDER BY name", conn);

                for (int i = 0; i < prefixes.Count; i++)
                    tableCmd.Parameters.AddWithValue($"@p{i}", prefixes[i] + "%");

                using (var reader = tableCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader["name"].ToString();
                        var matchedPrefix = prefixes.FirstOrDefault(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase));
                        list.Add(new SelectListItem
                        {
                            Text = name,
                            Value = name,
                            // Store prefix explicitly in Text OR as Value + use JS later
                            // We'll use this prefix in the view
                            Group = new SelectListGroup { Name = matchedPrefix } // optional: for optgroup use
                        });
                    }
                }
            }

            return list;
        }


        private List<(string Prefix, string Description)> GetTablePrefixes()
        {
            var list = new List<(string, string)>();
            using var conn = GetConnection();
            var cmd = new SqlCommand("SELECT Prefix, Description FROM foTablePrefixes WHERE Active = 1", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add((reader["Prefix"].ToString(), reader["Description"].ToString()));
            }
            return list;
        }


    }

}