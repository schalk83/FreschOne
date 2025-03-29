// CONTROLLER: foUserReportsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FreschOne.Models;

namespace FreschOne.Controllers
{
    public class foUserReportsController : BaseController
    {
        public foUserReportsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var list = new List<dynamic>();

            using (var conn = GetConnection())
            {
                string query = @"
                    SELECT ur.ID, ur.UserID, ur.ReportID,
                           u.FirstName + ' ' + u.LastName AS UserName,
                           r.ReportName
                    FROM foUserReports ur
                    INNER JOIN foUsers u ON ur.UserID = u.ID
                    INNER JOIN foReports r ON ur.ReportID = r.ID
                    WHERE ur.Active = 1";

                var cmd = new SqlCommand(query, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new
                    {
                        ID = (int)reader["ID"],
                        UserName = reader["UserName"].ToString(),
                        ReportName = reader["ReportName"].ToString()
                    });
                }
            }

            return View(list);
        }

        public IActionResult Edit(int id, int userid)
        {

            SetUserAccess(userid);
            ViewBag.userid = userid;

            foUserReports model = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foUserReports WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    model = new foUserReports
                    {
                        ID = (int)reader["ID"],
                        UserID = (long)reader["UserID"],
                        ReportID = (long)reader["ReportID"],
                        Active = (bool)reader["Active"]
                    };
                }
            }

            ViewBag.Users = GetUserSelectList();
            ViewBag.Reports = GetReportSelectList();

            return View(model); 
        }

        [HttpPost]
        public IActionResult Edit(foUserReports model, int userid)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = GetUserSelectList();
                ViewBag.Reports = GetReportSelectList();
                return View("Create", model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
            UPDATE foUserReports 
            SET UserID = @UserID, ReportID = @ReportID 
            WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@ReportID", model.ReportID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index",new { userid });
        }


        public IActionResult Create(int userid)
        {
            ViewBag.userid = userid;

            SetUserAccess(userid);

            ViewBag.Users = GetUserSelectList();
            ViewBag.Reports = GetReportSelectList();
            return View(new foUserReports());
        }

        [HttpPost]
        public IActionResult Create(foUserReports model, int userid)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = GetUserSelectList();
                ViewBag.Reports = GetReportSelectList();
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foUserReports (UserID, ReportID, Active) VALUES (@UserID, @ReportID, 1)", conn);
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@ReportID", model.ReportID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        private List<SelectListItem> GetUserSelectList()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, FirstName, LastName FROM foUsers WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["FirstName"] + " " + reader["LastName"]
                    });
                }
            }
            return list;
        }

        private List<SelectListItem> GetReportSelectList()
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
        public IActionResult Delete(int id, int userid)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var cmd = new SqlCommand("DELETE FROM foUserReports WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Assignment permanently deleted.";
            return RedirectToAction("Index", new { userid });
        }

    }
}