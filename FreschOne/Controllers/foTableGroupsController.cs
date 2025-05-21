using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;

namespace FreschOne.Controllers
{
    public class foTableGroupsController : BaseController
    {
        public foTableGroupsController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var reports = new List<foTableGroups>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Description FROM foTableGroups WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    reports.Add(new foTableGroups
                    {
                        ID = (long)reader["ID"],
                        Description = reader["Description"]?.ToString(),
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
        public IActionResult Create(foTableGroups report, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foTableGroups (Description, Active) VALUES (@Description, 1)", conn);
                cmd.Parameters.AddWithValue("@Description", report.Description ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foTableGroups report = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foTableGroups WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    report = new foTableGroups
                    {
                        ID = (long)reader["ID"],
                        Description = reader["Description"]?.ToString(),
                    };
                }
            }

            return View(report);
        }

        [HttpPost]
        public IActionResult Edit(foTableGroups report, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foTableGroups SET Description = @Description WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@Name", report.Description ?? (object)DBNull.Value);
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

                var checkTables = new SqlCommand("SELECT COUNT(*) FROM foTable WHERE TableGroup in ( select Description from foTableGroups WHERE ID = @ID )", conn);
                checkTables.Parameters.AddWithValue("@ID", id);
                int count1 = (int)checkTables.ExecuteScalar();


                if (count1 > 0 )
                {
                    TempData["ErrorMessage"] = "❌ This Group is in use and cannot be deleted : foTable";
                    return RedirectToAction("Index", new { userid });
                }

                var cmd = new SqlCommand("UPDATE foTableGroups SET Active = 0 WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Report deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }
    }
}
