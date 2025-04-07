using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Controllers
{
    public class foUserProcessController : BaseController
    {
        public foUserProcessController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;
            ViewBag.Users = GetUserSelectList(l_user ?? 0);

            var userProcesses = new List<foUserProcess>();

            if (l_user.HasValue)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand("SELECT up.ID, up.UserID, up.ProcessID, p.ProcessName " +
                        "FROM foUserProcess up " +
                        "JOIN foProcess p ON up.ProcessID = p.ID " +
                        "WHERE up.UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", l_user.Value);
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userProcesses.Add(new foUserProcess
                        {
                            ID = (long)reader["ID"],
                            UserID = (long)reader["UserID"],
                            ProcessID = (long)reader["ProcessID"],
                            ProcessName = reader["ProcessName"].ToString()

                        });
                    }
                }
            }

            return View(userProcesses);
        }


        public IActionResult Create(int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            // Populate unassigned processes for dropdown
            var processes = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT ID, ProcessName FROM foProcess
                    WHERE Active = 1 AND ID NOT IN (
                        SELECT ProcessID FROM foUserProcess WHERE UserID = @UserID
                    )", conn);
                cmd.Parameters.AddWithValue("@UserID", l_user);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    processes.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["ProcessName"].ToString()
                    });
                }
            }
            ViewBag.ProcessList = processes;

            return View();
        }

        [HttpPost]
        public IActionResult Create(foUserProcess model, int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;



            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foUserProcess (UserID, ProcessID) VALUES (@UserID, @ProcessID)", conn);
                cmd.Parameters.AddWithValue("@UserID", l_user);
                cmd.Parameters.AddWithValue("@ProcessID", model.ProcessID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user });
        }

        public IActionResult Delete(long id, int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foUserProcess WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user });
        }

        private List<SelectListItem> GetUserSelectList(long selectedId = 0)
        {
            var list = new List<SelectListItem>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("SELECT ID, FirstName, LastName FROM foUsers WHERE Active = 1", conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var id = Convert.ToInt64(reader["ID"]);
                    list.Add(new SelectListItem
                    {
                        Value = id.ToString(),
                        Text = reader["FirstName"] + " " + reader["LastName"],
                        Selected = id == selectedId
                    });
                }
            }

            return list.OrderBy(x => x.Text).ToList();
        }
    }
}
