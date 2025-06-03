using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Controllers
{
    public class foReportAccessController : BaseController
    {
        public foReportAccessController(DatabaseHelper dbHelper, IConfiguration configuration)
            : base(dbHelper, configuration) { }

        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult Index(int userid, long reportid = 0)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            ViewBag.Users = GetFilteredUsersForDropdown(reportid);
            ViewBag.Groups = GetFilteredGroupsForDropdown(reportid);
            ViewBag.Reports = GetReportsForDropdown(reportid);

            var accessList = new List<foReportAccess>();

            if (reportid > 0)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand(@"
                        SELECT a.ID, a.ReportID, a.UserID, 
                               u.FirstName + ' ' + u.LastName AS UserName,
                               a.GroupID, g.Description AS GroupName, a.Active
                        FROM foReportAccess a
                        LEFT JOIN foUsers u ON a.UserID = u.ID
                        LEFT JOIN foGroups g ON a.GroupID = g.ID
                        WHERE a.Active = 1 AND a.ReportID = @reportid", conn);

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
                                UserName = reader["UserName"] as string ?? "-",
                                GroupName = reader["GroupName"] as string ?? "-",
                                Active = (bool)reader["Active"]
                            });
                        }
                    }
                }
            }

            ViewBag.AccessList = accessList;

            return View(new foReportAccess());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection form, int userid, long reportid)
        {
            SetUserAccess(userid);
            ViewBag.userid = userid;
            ViewBag.reportid = reportid;

            // Extract values from form
            long? accessUserID = string.IsNullOrEmpty(form["accessUserID"]) ? null : long.Parse(form["accessUserID"]);
            long? groupID = string.IsNullOrEmpty(form["GroupID"]) ? null : long.Parse(form["GroupID"]);

            ViewBag.Users = GetFilteredUsersForDropdown(reportid, accessUserID);
            ViewBag.Groups = GetFilteredGroupsForDropdown(reportid, groupID);
            ViewBag.Reports = GetReportsForDropdown(reportid);

            var accessList = new List<foReportAccess>();

            if (reportid > 0)
            {
                using (var conn = GetConnection())
                {
                    var cmd = new SqlCommand(@"
                        SELECT a.ID, a.UserID, u.FirstName + ' ' + u.LastName AS UserName,
                               a.GroupID, g.Description AS GroupName, a.Active
                        FROM foReportAccess a
                        LEFT JOIN foUsers u ON a.UserID = u.ID
                        LEFT JOIN foGroups g ON a.GroupID = g.ID
                        WHERE a.Active = 1 AND a.ReportID = @reportid", conn);

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
                                UserName = reader["UserName"] as string ?? "-",
                                GroupName = reader["GroupName"] as string ?? "-",
                                Active = (bool)reader["Active"]
                            });
                        }
                    }
                }
            }

            ViewBag.AccessList = accessList;

            if (!accessUserID.HasValue && !groupID.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Please select either a User or a Group.");
            }

            if (reportid == 0)
            {
                ModelState.AddModelError("ReportID", "Please select a Report.");
            }

            if (!ModelState.IsValid)
            {
                var formModel = new foReportAccess
                {
                    ReportID = reportid,
                    UserID = accessUserID,
                    GroupID = groupID
                };
                return View("Index", formModel);
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO foReportAccess (ReportID, UserID, GroupID, Active) 
                    VALUES (@ReportID, @UserID, @GroupID, 1)", conn);
                cmd.Parameters.AddWithValue("@ReportID", reportid);
                cmd.Parameters.AddWithValue("@UserID", (object)accessUserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GroupID", (object)groupID ?? DBNull.Value);
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

        private List<SelectListItem> GetFilteredUsersForDropdown(long reportid, long? selectedUserId = null)
        {
            var assignedUserIds = new HashSet<long>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DISTINCT UserID FROM foReportAccess WHERE Active = 1 AND ReportID = @reportid AND UserID IS NOT NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@reportid", reportid);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            assignedUserIds.Add((long)reader["UserID"]);
                        }
                    }
                }
            }

            if (selectedUserId.HasValue)
            {
                assignedUserIds.Remove(selectedUserId.Value);
            }

            var users = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers WHERE Active = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = (long)reader["ID"];
                        if (!assignedUserIds.Contains(id))
                        {
                            users.Add(new SelectListItem
                            {
                                Value = id.ToString(),
                                Text = reader["FullName"].ToString(),
                                Selected = (selectedUserId.HasValue && id == selectedUserId)
                            });
                        }
                    }
                }
            }
            return users;
        }

        private List<SelectListItem> GetFilteredGroupsForDropdown(long reportid, long? selectedGroupId = null)
        {
            var assignedGroupIds = new HashSet<long>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT DISTINCT GroupID FROM foReportAccess WHERE Active = 1 AND ReportID = @reportid AND GroupID IS NOT NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@reportid", reportid);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            assignedGroupIds.Add((long)reader["GroupID"]);
                        }
                    }
                }
            }

            if (selectedGroupId.HasValue)
            {
                assignedGroupIds.Remove(selectedGroupId.Value);
            }

            var groups = new List<SelectListItem>();
            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT ID, Description FROM foGroups WHERE Active = 1", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = (long)reader["ID"];
                        if (!assignedGroupIds.Contains(id))
                        {
                            groups.Add(new SelectListItem
                            {
                                Value = id.ToString(),
                                Text = reader["Description"].ToString(),
                                Selected = (selectedGroupId.HasValue && id == selectedGroupId)
                            });
                        }
                    }
                }
            }
            return groups;
        }

        private List<SelectListItem> GetReportsForDropdown(long selectedReportId = 0)
        {
            var reports = new List<SelectListItem>();

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ID, ReportName FROM foReports WHERE Active = 1", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var reportId = (long)reader["ID"];
                        reports.Add(new SelectListItem
                        {
                            Value = reportId.ToString(),
                            Text = reader["ReportName"].ToString(),
                            Selected = (reportId == selectedReportId)
                        });
                    }
                }
            }

            return reports;
        }
    }
}
