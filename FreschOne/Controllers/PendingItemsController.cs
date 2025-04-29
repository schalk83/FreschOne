using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class PendingItemsController : BaseController
    {
        public PendingItemsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult MergedPendingItems(int userId)
        {
            SetUserAccess(userId);
            var pendingSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                // Fetch Pending Process Steps
                string processQuery = @"
                    SELECT 
                        e.ID,
                        e.ProcessInstanceID,
                        ISNULL(e.StepID, 0) AS StepID,
                        e.GroupID,
                        g.Description AS GroupDescription,
                        e.UserID,
                        u.FirstName,
                        u.LastName,
                        e.DateAssigned,
                        s.StepDescription,
                        ISNULL(s.StepNo, 1) AS StepNo,
                        ISNULL(s.ProcessID, 0) AS ProcessID
                    FROM foProcessEvents e
                    JOIN foProcessSteps s ON e.StepID = s.ID
                    LEFT JOIN foGroups g ON e.GroupID = g.ID
                    LEFT JOIN foUsers u ON e.UserID = u.ID
                    WHERE e.DateCompleted IS NULL
                      AND (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
                ";

                using (var cmd = new SqlCommand(processQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7)
                                    ? null
                                    : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.GetDateTime(8),
                                StepDescription = reader.GetString(9),
                                StepNo = Convert.ToDouble(reader["StepNo"]),
                                ProcessID = Convert.ToInt32(reader["ProcessID"]),
                                StepType = "Process" // <--- Important
                            });
                        }
                    }
                }

                // Fetch Pending Approval Steps
                string approvalQuery = @"
                    SELECT 
                        e.ID,
                        e.ProcessInstanceID,
                        ISNULL(e.StepID, 0) AS StepID,
                        e.GroupID,
                        g.Description AS GroupDescription,
                        e.UserID,
                        u.FirstName,
                        u.LastName,
                        e.DateAssigned,
                        COALESCE(s.StepDescription, 'Approval required for ' + p.ProcessDescription) AS StepDescription,
                        ISNULL(s.StepNo, 1) AS StepNo,
                        ISNULL(s.ProcessID, 0) AS ProcessID
                    FROM foApprovalEvents e
                    LEFT JOIN foApprovalSteps s ON e.StepID = s.ID
                    LEFT JOIN foGroups g ON e.GroupID = g.ID
                    LEFT JOIN foUsers u ON e.UserID = u.ID
                    LEFT JOIN (
                        SELECT pe.ID AS EventID, p.ProcessDescription
                        FROM foProcessEvents pe
                        INNER JOIN foProcessSteps ps ON pe.StepID = ps.ID
                        INNER JOIN foProcess p ON ps.ProcessID = p.ID
                    ) p ON p.EventID = ABS(e.PreviousEventID)
                    WHERE e.DateCompleted IS NULL
                      AND (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
                ";

                using (var cmd = new SqlCommand(approvalQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pendingSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7)
                                    ? null
                                    : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.GetDateTime(8),
                                StepDescription = reader.GetString(9),
                                StepNo = Convert.ToDouble(reader["StepNo"]),
                                ProcessID = Convert.ToInt32(reader["ProcessID"]),
                                StepType = "Approval" // <--- Important
                            });
                        }
                    }
                }
            }

            // Sort the merged list by DateAssigned DESC (newest first)
            pendingSteps = pendingSteps.OrderByDescending(x => x.DateAssigned).ToList();

            // Get User's Full Name for Title
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand("SELECT FirstName + ' ' + LastName FROM foUsers WHERE ID = @ID", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ID", userId);
                ViewBag.FullName = cmd.ExecuteScalar()?.ToString() ?? "";
            }

            ViewBag.UserID = userId;
            return View(pendingSteps); // Will look for /Views/PendingItems/MergedPendingItems.cshtml
        }
    }
}
