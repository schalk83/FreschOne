using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Controllers
{
    public class foReportAccessController : BaseController
    {
        public foReportAccessController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long reportid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            var accessList = new List<foReportAccess>();

            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"
                    SELECT ID, ReportID, UserID, GroupID, Active 
                    FROM foReportAccess 
                    WHERE Active = 1 AND ReportID = @reportid", conn);
                cmd.Parameters.AddWithValue("@reportid", reportid);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessList.Add(new foReportAccess
                        {
                            ID = (long)reader["ID"],
                            ReportID = (long)reader["ReportID"],
                            UserID = reader["UserID"] as long?,
                            GroupID = reader["GroupID"] as long?,
                            Active = (bool)reader["Active"]
                        });
                    }
                }
            }

            ViewBag.Users = GetUsersForDropdown();
            ViewBag.Groups = GetGroupsForDropdown();

            return View(accessList);
        }

        public IActionResult Create(int userid, long reportid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            var model = new foReportAccess { ReportID = reportid };

            ViewBag.Users = GetUsersForDropdown();
            ViewBag.Groups = GetGroupsForDropdown();

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(foReportAccess model, int userid, long reportid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            ViewBag.Users = GetUsersForDropdown();
            ViewBag.Groups = GetGroupsForDropdown();

            // Load AccessList (this is your existing logic!)
            var accessList = new List<foReportAccess>();
            using (var conn = GetConnection())
            {
                var cmd = new SqlCommand(@"SELECT ID, UserID, GroupID, Active FROM foReportAccess WHERE Active = 1 AND ReportID = @reportid", conn);
                cmd.Parameters.AddWithValue("@reportid", reportid);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessList.Add(new foReportAccess
                        {
                            ID = (long)reader["ID"],
                            ReportID = reportid,
                            UserID = reader["UserID"] as long?,
                            GroupID = reader["GroupID"] as long?,
                            Active = (bool)reader["Active"]
                        });
                    }
                }
            }
            ViewBag.AccessList = accessList;

            model.ReportID = reportid;

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
        INSERT INTO foReportAccess (ReportID, UserID, GroupID, Active) 
        VALUES (@ReportID, @UserID, @GroupID, 1)", conn);
                cmd.Parameters.AddWithValue("@ReportID", reportid);
                cmd.Parameters.AddWithValue("@UserID", (object)model.UserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GroupID", (object)model.GroupID ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }

            TempData["ReportAccessMessage"] = "Access added successfully.";
            return RedirectToAction("Index", new { userid, reportid });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(long id, int userid, long reportid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE foReportAccess SET Active = 0 WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", new { userid, reportid });
        }


        private List<SelectListItem> GetUsersForDropdown()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers WHERE Active = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["FullName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private List<SelectListItem> GetGroupsForDropdown()
        {
            var list = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID, Description FROM foGroups WHERE Active = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = reader["Description"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}
