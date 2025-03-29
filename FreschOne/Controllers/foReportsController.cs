using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;

namespace FreschOne.Controllers
{
    public class foReportsController : BaseController
    {
        public foReportsController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var reports = new List<foReports>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, ReportName, ReportDescription FROM foReports WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reports.Add(new foReports
                    {
                        ID = (long)reader["ID"],
                        ReportName = reader["ReportName"]?.ToString(),
                        ReportDescription = reader["ReportDescription"]?.ToString()
                    });
                }
            }

            return View(reports);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            return View();
        }

        [HttpPost]
        public IActionResult Create(foReports report, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foReports (ReportName, ReportDescription, Active) VALUES (@Name, @Desc, 1)", conn);
                cmd.Parameters.AddWithValue("@Name", report.ReportName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Desc", report.ReportDescription ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foReports report = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foReports WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    report = new foReports
                    {
                        ID = (long)reader["ID"],
                        ReportName = reader["ReportName"]?.ToString(),
                        ReportDescription = reader["ReportDescription"]?.ToString()
                    };
                }
            }

            return View(report);
        }

        [HttpPost]
        public IActionResult Edit(foReports report, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foReports SET ReportName = @Name, ReportDescription = @Desc WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@Name", report.ReportName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Desc", report.ReportDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", report.ID);

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
                conn.Open();

                var checkUserReports = new SqlCommand("SELECT COUNT(*) FROM foUserReports WHERE ReportID = @ID", conn);
                checkUserReports.Parameters.AddWithValue("@ID", id);
                int count1 = (int)checkUserReports.ExecuteScalar();

                var checkReportTable = new SqlCommand("SELECT COUNT(*) FROM foReportTable WHERE ReportsID = @ID", conn);
                checkReportTable.Parameters.AddWithValue("@ID", id);
                int count2 = (int)checkReportTable.ExecuteScalar();

                if (count1 > 0 || count2 > 0)
                {
                    TempData["ErrorMessage"] = "❌ This report is in use and cannot be deleted : foUserReports, foReportTable ";
                    return RedirectToAction("Index", new { userid });
                }

                var cmd = new SqlCommand("UPDATE foReports SET Active = 0 WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Report deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }
    }
}
