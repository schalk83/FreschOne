using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using FreschOne.Models;

namespace FreschOne.Controllers
{
    public class foUserReportsController : BaseController
    {
        public foUserReportsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() =>
            new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long? l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;
            ViewBag.Users = GetUserSelectList(l_user ?? 0);

            var list = new List<dynamic>();

            if (l_user.HasValue)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand(@"
                        SELECT ur.ID, ur.UserID, ur.ReportID,
                               u.FirstName + ' ' + u.LastName AS UserName,
                               r.ReportName
                        FROM foUserReports ur
                        INNER JOIN foUsers u ON ur.UserID = u.ID
                        INNER JOIN foReports r ON ur.ReportID = r.ID
                        WHERE ur.Active = 1 AND ur.UserID = @UserID", conn);

                    cmd.Parameters.AddWithValue("@UserID", l_user);
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
            }

            return View(list);
        }

        public IActionResult Create(int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;
            ViewBag.Reports = GetReportSelectList(l_user);

            return View(new foUserReports { UserID = l_user });
        }

        [HttpPost]
        public IActionResult Create(foUserReports model, int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            if (!ModelState.IsValid)
            {
                ViewBag.Reports = GetReportSelectList(l_user);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("INSERT INTO foUserReports (UserID, ReportID, Active) VALUES (@UserID, @ReportID, 1)", conn);
                cmd.Parameters.AddWithValue("@UserID", l_user);
                cmd.Parameters.AddWithValue("@ReportID", model.ReportID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user });
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

            ViewBag.l_user = model?.UserID;
            ViewBag.Reports = GetReportSelectList(model.UserID, model.ReportID);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(foUserReports model, int userid, long l_user)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.l_user = l_user;

            var exists = false;

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM foUserReports 
                    WHERE UserID = @UserID AND ReportID = @ReportID AND ID <> @ID AND Active = 1", conn);

                cmd.Parameters.AddWithValue("@UserID", l_user);
                cmd.Parameters.AddWithValue("@ReportID", model.ReportID);
                cmd.Parameters.AddWithValue("@ID", model.ID);

                conn.Open();
                exists = (int)cmd.ExecuteScalar() > 0;
            }

            if (exists)
            {
                ModelState.AddModelError("ReportID", "This report is already assigned to the user.");
                ViewBag.Reports = GetReportSelectList(l_user, model.ReportID);
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Reports = GetReportSelectList(l_user);
                return View(model);
            }

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("UPDATE foUserReports SET ReportID = @ReportID WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", model.ID);
                cmd.Parameters.AddWithValue("@ReportID", model.ReportID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, l_user });
        }

        [HttpPost]
        public IActionResult Delete(int id, int userid, long l_user)
        {
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand("DELETE FROM foUserReports WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "✅ Assignment permanently deleted.";
            return RedirectToAction("Index", new { userid, l_user });
        }

        private List<SelectListItem> GetReportSelectList(long userId, long? currentReportId = null)
        {
            var list = new List<SelectListItem>();
            var assignedIds = new HashSet<long>();

            using (var conn = GetConnection())
            {
                var assignedCmd = new SqlCommand(@"
                    SELECT ReportID FROM foUserReports 
                    WHERE UserID = @UserID AND Active = 1" +
                    (currentReportId.HasValue ? " AND ReportID <> @CurrentReportID" : ""), conn);

                assignedCmd.Parameters.AddWithValue("@UserID", userId);
                if (currentReportId.HasValue)
                    assignedCmd.Parameters.AddWithValue("@CurrentReportID", currentReportId.Value);

                conn.Open();
                using (var reader = assignedCmd.ExecuteReader())
                {
                    while (reader.Read())
                        assignedIds.Add((long)reader["ReportID"]);
                }

                var cmd = new SqlCommand("SELECT ID, ReportName FROM foReports WHERE Active = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = (long)reader["ID"];
                        if (!assignedIds.Contains(id) || (currentReportId.HasValue && id == currentReportId.Value))
                        {
                            list.Add(new SelectListItem
                            {
                                Value = id.ToString(),
                                Text = reader["ReportName"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
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
