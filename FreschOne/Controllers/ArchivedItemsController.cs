using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class ArchivedItemsController : BaseController
    {
        public ArchivedItemsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult ArchivedItems(int userId)
        {
            SetUserAccess(userId);
            var archivedSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                // Fetch Archived Process Steps (last step per ProcessInstanceID)
                string processQuery = @"
                    SELECT 
                        e.ID,
                        e.ProcessInstanceID,
                        e.StepID,
                        e.GroupID,
                        g.Description AS GroupDescription,
                        e.UserID,
                        u.FirstName,
                        u.LastName,
                        e.DateCompleted,
                        s.StepDescription,
                        s.StepNo,
                        s.ProcessID
                    FROM foProcessEventsArchive e
                    LEFT JOIN foProcessSteps s ON e.StepID = s.ID
                    LEFT JOIN foProcess p ON s.ProcessID = p.ID
                    LEFT JOIN foGroups g ON e.GroupID = g.ID
                    LEFT JOIN foUsers u ON e.UserID = u.ID
                    WHERE (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
                      AND e.ID = (SELECT TOP 1 ID FROM foProcessEventsArchive WHERE ProcessInstanceID = e.ProcessInstanceID ORDER BY StepNo DESC, DateAssigned DESC)
                ";

                using (var cmd = new SqlCommand(processQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            archivedSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7)
        ? null
        : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                StepNo = reader.IsDBNull(10) ? 0 : Convert.ToDouble(reader["StepNo"]),
                                ProcessID = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader["ProcessID"]),
                                StepType = "Process" // or "Approval" for the other block
                            });

                        }
                    }
                }

                // Fetch Archived Approval Steps (last step per ProcessInstanceID)
                string approvalQuery = @"
                    SELECT 
                        e.ID,
                        e.ProcessInstanceID,
                        e.StepID,
                        e.GroupID,
                        g.Description AS GroupDescription,
                        e.UserID,
                        u.FirstName,
                        u.LastName,
                        e.DateCompleted,
                        COALESCE(s.StepDescription, 'Approval required for ' + p.ProcessDescription) AS StepDescription,
                        s.StepNo,
                        s.ProcessID
                    FROM foApprovalEventsArchive e
                    LEFT JOIN foApprovalSteps s ON e.StepID = s.ID
                    LEFT JOIN foProcess p ON s.ProcessID = p.ID
                    LEFT JOIN foGroups g ON e.GroupID = g.ID
                    LEFT JOIN foUsers u ON e.UserID = u.ID
                    WHERE (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
                      AND e.ID = (SELECT TOP 1 ID FROM foApprovalEventsArchive WHERE ProcessInstanceID = e.ProcessInstanceID ORDER BY StepNo DESC, DateAssigned DESC)
                ";

                using (var cmd = new SqlCommand(approvalQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            archivedSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                            FullName = reader.IsDBNull(6) && reader.IsDBNull(7)
                                ? null
                                : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                StepNo = reader.IsDBNull(10) ? 0 : Convert.ToDouble(reader["StepNo"]),
                                ProcessID = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader["ProcessID"]),
                                StepType = "Process" // or "Approval" for the other block
                            });
                        }
                    }
                }
            }

            // Sort archived list by DateAssigned DESC (latest first)
            archivedSteps = archivedSteps.OrderByDescending(x => x.DateAssigned).ToList();

            // Get User's Full Name for Title
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand("SELECT FirstName + ' ' + LastName FROM foUsers WHERE ID = @ID", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ID", userId);
                ViewBag.FullName = cmd.ExecuteScalar()?.ToString() ?? "";
            }

            ViewBag.UserID = userId;
            return View(archivedSteps); // Will look for /Views/ArchivedItems/MergedArchivedItems.cshtml
        }
    }
}
