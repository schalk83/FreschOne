using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;

namespace FreschOne.Controllers
{
    public class foGroupsController : BaseController
    {
        public foGroupsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            var groups = new List<foGroups>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, Description FROM foGroups", conn);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add(new foGroups
                    {
                        ID = (long)reader["ID"],
                        Description = reader["Description"]?.ToString()
                    });
                }
            }

            return View(groups);
        }

        public IActionResult Create(int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            return View();
        }

        [HttpPost]
        public IActionResult Create(foGroups group, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foGroups (Description) VALUES (@Description)", conn);
                cmd.Parameters.AddWithValue("@Description", group.Description ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid });
        }

        public IActionResult Edit(long id, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            foGroups group = null;
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT * FROM foGroups WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    group = new foGroups
                    {
                        ID = (long)reader["ID"],
                        Description = reader["Description"]?.ToString()
                    };
                }
            }

            return View(group);
        }

        [HttpPost]
        public IActionResult Edit(foGroups group, int userid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foGroups SET Description = @Description WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@Description", group.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ID", group.ID);
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

                // Check usage in other tables (future proofing)
                var checkUsage = new SqlCommand("SELECT COUNT(*) FROM foUsers WHERE GroupID = @ID", conn);
                checkUsage.Parameters.AddWithValue("@ID", id);
                int count = (int)checkUsage.ExecuteScalar();

                if (count > 0)
                {
                    TempData["ErrorMessage"] = "❌ This group is in use and cannot be deleted.";
                    return RedirectToAction("Index", new { userid });
                }

                var cmd = new SqlCommand("DELETE FROM foGroups WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();

                TempData["Message"] = "✅ Group deleted successfully.";
            }

            return RedirectToAction("Index", new { userid });
        }
    }
}