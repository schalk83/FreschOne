using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;
using System.Diagnostics;

namespace FreschOne.Controllers
{
    public class foProcessController : BaseController
    {
        public foProcessController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var processes = new List<foProcess>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, ProcessName, ProcessDescription FROM foProcess WHERE Active = 1", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    processes.Add(new foProcess
                    {
                        ID = (long)reader["ID"],
                        ProcessName = reader["ProcessName"]?.ToString(),
                        ProcessDescription = reader["ProcessDescription"]?.ToString()
                    });
                }
            }

            return View(processes);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            return View();
        }

        [HttpPost]
        public IActionResult Create(foProcess process, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foProcess (ProcessName, ProcessDescription, Active) VALUES (@Name, @Desc, 1)", conn);
                cmd.Parameters.AddWithValue("@Name", process.ProcessName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Desc", process.ProcessDescription ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foProcess process = null;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foProcess WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    process = new foProcess
                    {
                        ID = (long)reader["ID"],
                        ProcessName = reader["ProcessName"]?.ToString(),
                        ProcessDescription = reader["ProcessDescription"]?.ToString()
                    };
                }
            }

            return View(process);
        }

        [HttpPost]
        public IActionResult Edit(foProcess process, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foProcess SET ProcessName = @Name, ProcessDescription = @Desc WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@Name", process.ProcessName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Desc", process.ProcessDescription ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", process.ID);

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

                var checkUserProcess = new SqlCommand("SELECT COUNT(*) FROM foUserProcess WHERE ProcessID = @ID", conn);
                checkUserProcess.Parameters.AddWithValue("@ID", id);
                int count1 = (int)checkUserProcess.ExecuteScalar();

                var checkProcessSteps = new SqlCommand("SELECT COUNT(*) FROM foProcessSteps WHERE ProcessID = @ID", conn);
                checkProcessSteps.Parameters.AddWithValue("@ID", id);
                int count2 = (int)checkProcessSteps.ExecuteScalar();

                if (count1 > 0 || count2 > 0)
                {
                    TempData["ErrorMessage"] = "❌ This process is in use and cannot be deleted: foUserProcess, foProcessSteps";
                    return RedirectToAction("Index", new { userid });
                }

                var cmd = new SqlCommand("UPDATE foProcess SET Active = 0 WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Process deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }
    }
}
