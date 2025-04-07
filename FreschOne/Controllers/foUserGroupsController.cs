using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using FreschOne.Models;

namespace FreschOne.Controllers
{
    public class foUserGroupsController : BaseController
    {
        public foUserGroupsController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;
            ViewBag.Users = GetUserSelectList(l_user ?? 0);

            var userGroups = new List<foUserGroups>();

            if (l_user.HasValue)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand("SELECT ug.ID, ug.UserID, ug.GroupID, g.Description AS GroupName " +
                                            "FROM foUserGroups ug " +
                                            "JOIN foGroups g ON ug.GroupID = g.ID " +
                                            "WHERE ug.UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", l_user.Value);
                    conn.Open();
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        userGroups.Add(new foUserGroups
                        {
                            ID = (long)reader["ID"],
                            UserID = (long)reader["UserID"],
                            GroupID = (long)reader["GroupID"],
                            GroupName = reader["GroupName"].ToString()
                        });
                    }
                }
            }

            return View(userGroups);
        }

        public IActionResult Create(int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            // Populate unassigned groups for dropdown
            var groups = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT ID, Description FROM foGroups
                    WHERE ID NOT IN (
                        SELECT GroupID FROM foUserGroups WHERE UserID = @UserID
                    )", conn);
                cmd.Parameters.AddWithValue("@UserID", l_user);
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    groups.Add(new SelectListItem
                    {
                        Value = reader["ID"].ToString(),
                        Text = reader["Description"].ToString()
                    });
                }
            }
            ViewBag.GroupList = groups.OrderBy(x => x.Text).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(foUserGroups model, int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foUserGroups (UserID, GroupID) VALUES (@UserID, @GroupID)", conn);
                cmd.Parameters.AddWithValue("@UserID", l_user);
                cmd.Parameters.AddWithValue("@GroupID", model.GroupID);
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
                var cmd = new SqlCommand("DELETE FROM foUserGroups WHERE ID = @ID", conn);
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
