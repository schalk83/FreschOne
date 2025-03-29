
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FreschOne.Models;
using System.Formats.Tar;

namespace FreschOne.Controllers
{
    public class foReportTableController : BaseController
    {
        public foReportTableController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(long? reportid, int userid)
        {

            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.ReportID = reportid;
            ViewBag.ReportsDropdown = GetReportsSelectList();

            var list = new List<foReportTable>();

            if (reportid == null)
                return View(list);

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                SELECT * FROM foReportTable 
                WHERE ReportsID = @ReportsID AND Active = 1", conn);
                cmd.Parameters.AddWithValue("@ReportsID", reportid);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new foReportTable
                    {
                        ID = (long)reader["ID"],
                        ReportsID = (long)reader["ReportsID"],
                        TableName = reader["TableName"]?.ToString(),
                        ColumnQuery = reader["ColumnQuery"]?.ToString(),
                        FormType = reader["FormType"]?.ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        Parent = Convert.ToBoolean(reader["Parent"]),
                        FKColumn = reader["FKColumn"]?.ToString(),
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

            return View(new foReportTable { ReportsID = reportid });
        }

        [HttpPost]
        public IActionResult Create(foReportTable model, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!model.Parent && string.IsNullOrWhiteSpace(model.FKColumn))
            {
                ModelState.AddModelError("FKColumn", "Foreign Key Column is required when Parent is No.");
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
                INSERT INTO foReportTable 
                (ReportsID, TableName, ColumnQuery, FormType, ColumnCount, Parent, FKColumn, TableDescription, Active)
                VALUES 
                (@ReportsID, @TableName, @ColumnQuery, @FormType, @ColumnCount, @Parent, @FKColumn, @TableDescription, 1)", conn);

                cmd.Parameters.AddWithValue("@ReportsID", model.ReportsID);
                cmd.Parameters.AddWithValue("@TableName", (object?)model.TableName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnQuery", (object?)model.ColumnQuery ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", (object?)model.FormType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", (object?)model.ColumnCount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Parent", model.Parent);
                cmd.Parameters.AddWithValue("@FKColumn", (object?)model.FKColumn ?? DBNull.Value);
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

            foReportTable model = null;
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foReportTable WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foReportTable
                    {
                        ID = (long)reader["ID"],
                        ReportsID = (long)reader["ReportsID"],
                        TableName = reader["TableName"]?.ToString(),
                        ColumnQuery = reader["ColumnQuery"]?.ToString(),
                        FormType = reader["FormType"]?.ToString(),
                        ColumnCount = reader["ColumnCount"] as int?,
                        Parent = Convert.ToBoolean(reader["Parent"]),
                        FKColumn = reader["FKColumn"]?.ToString(),
                        TableDescription = reader["TableDescription"]?.ToString(),
                        Active = reader["Active"] as bool?
                    };
                }
            }

            ViewBag.FormTypes = GetFormTypeSelectList();
            ViewBag.ReportID = model.ReportsID;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foReportTable model, int userid)
        {

            
            if (Request.Form.ContainsKey("Parent"))
            {
                model.Parent = Request.Form["Parent"].Contains("true");
            }

            SetUserAccess(userid);
            ViewBag.userid = userid;

            if (!model.Parent && string.IsNullOrWhiteSpace(model.FKColumn))
            {
                ModelState.AddModelError("FKColumn", "Foreign Key Column is required when Parent is No.");
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
                UPDATE foReportTable 
                SET TableName = @TableName, ColumnQuery = @ColumnQuery, FormType = @FormType, ColumnCount = @ColumnCount,
                    Parent = @Parent, FKColumn = @FKColumn, TableDescription = @TableDescription
                WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@TableName", (object?)model.TableName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnQuery", (object?)model.ColumnQuery ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FormType", (object?)model.FormType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ColumnCount", (object?)model.ColumnCount ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Parent", model.Parent);
                cmd.Parameters.AddWithValue("@FKColumn", (object?)model.FKColumn ?? DBNull.Value);
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
                var cmd = new SqlCommand("DELETE FROM foReportTable WHERE ID = @ID", conn);
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

        [HttpPost]
        public IActionResult CheckParent(long reportid, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            bool hasParent = false;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT COUNT(*) FROM foReportTable WHERE ReportsID = @ReportID AND Parent = 1 AND Active = 1", conn);
                cmd.Parameters.AddWithValue("@ReportID", reportid);
                conn.Open();
                var count = (int)cmd.ExecuteScalar();
                hasParent = count > 0;
            }

            if (!hasParent)
            {
                TempData["ErrorMessage"] = "❌ You must define at least one Parent table for this report.";
            }
            else
            {
                TempData["Message"] = "✅ A Parent table exists.";
            }

            return RedirectToAction("Index", new { reportid, userid });
        }


        private List<SelectListItem> GetFormTypeSelectList()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Value = "F", Text = "F" },
            new SelectListItem { Value = "T", Text = "T" }
        };
        }
    }

}