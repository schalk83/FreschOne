using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class PendingItemsController : BaseController
    {
        public PendingItemsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult MergedPendingItems(int userId, int pageNumber = 1, int pageSize = 10, string processnumberFilter = "", string processNameFilter = "", string stepNoFilter = "", string stepDescriptionFilter = "", string assignedToFilter = "", string dateFilter = "")
        {
            SetUserAccess(userId);
            var pendingSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                // Fetch Process Steps
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
                e.DateAssigned,
                s.StepDescription,
                ISNULL(s.StepNo, 1) AS StepNo,
                ISNULL(s.ProcessID, 0) AS ProcessID,
                p.ProcessName
            FROM foProcessEvents e
            JOIN foProcessSteps s ON e.StepID = s.ID
            JOIN foProcess p ON s.ProcessID = p.ID
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
                            var assignedTo = (reader.IsDBNull(6) && reader.IsDBNull(7)) ? (reader.IsDBNull(4) ? "" : reader.GetString(4)) : $"{reader.GetString(6)} {reader.GetString(7)}";

                            pendingSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                FullName = (reader.IsDBNull(6) && reader.IsDBNull(7)) ? null : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "Adhoc Approval" : reader.GetString(9),
                                StepNo = Convert.ToDouble(reader["StepNo"]),
                                ProcessID = Convert.ToInt32(reader["ProcessID"]),
                                ProcessName = reader["ProcessName"].ToString(),
                                StepType = "Process"
                            });
                        }
                    }
                }

                // Fetch Approval Steps
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
                e.DateAssigned,

                COALESCE(
                    ps.StepDescription, 
                   'Adhoc approval ' +  fallbackPs.StepDescription, 
                    'Approval required'
                ) AS StepDescription,

                ISNULL(s.StepNo, fallbackPs.StepNo) AS StepNo,
                ISNULL(s.ProcessID, fallbackPs.ProcessID) AS ProcessID,
                p.ProcessName

            FROM foApprovalEvents e

            -- Normal link to ApprovalSteps
            LEFT JOIN foApprovalSteps s ON e.StepID = s.ID
            LEFT JOIN foProcess p ON s.ProcessID = p.ID
            LEFT JOIN foProcessSteps ps ON ps.ProcessID = s.ProcessID AND ps.StepNo = s.StepNo

            -- Fallback link: get the last process step if StepID = 0 (via foProcessEvents)
            LEFT JOIN foProcessEvents pe ON pe.ProcessInstanceID = e.ProcessInstanceID AND pe.StepID <> 0
            LEFT JOIN foProcessSteps fallbackPs ON fallbackPs.ID = pe.StepID

            LEFT JOIN foGroups g ON e.GroupID = g.ID
            LEFT JOIN foUsers u ON e.UserID = u.ID

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
                            var assignedTo = (reader.IsDBNull(6) && reader.IsDBNull(7)) ? (reader.IsDBNull(4) ? "" : reader.GetString(4)) : $"{reader.GetString(6)} {reader.GetString(7)}";

                            pendingSteps.Add(new PendingStepViewModel
                            {
                                EventID = reader.GetInt32(0),
                                ProcessInstanceID = reader.GetInt64(1),
                                StepID = reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                FullName = (reader.IsDBNull(6) && reader.IsDBNull(7)) ? null : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "Adhoc Approval" : reader.GetString(9),
                                StepNo = Convert.ToDouble(reader["StepNo"]),
                                ProcessID = Convert.ToInt32(reader["ProcessID"]),
                                ProcessName = reader["ProcessName"].ToString(),
                                StepType = "Approval"
                            });
                        }
                    }
                }
            }

            // Apply search filters
            if (!string.IsNullOrEmpty(processnumberFilter))
                pendingSteps = pendingSteps.Where(x => x.ProcessInstanceID.ToString().Contains(processnumberFilter)).ToList();
            if (!string.IsNullOrEmpty(processNameFilter))
                pendingSteps = pendingSteps.Where(x => x.ProcessName?.IndexOf(processNameFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (!string.IsNullOrEmpty(stepNoFilter))
                pendingSteps = pendingSteps.Where(x => x.StepNo.ToString().Contains(stepNoFilter)).ToList();
            pendingSteps = pendingSteps.Where(x => x.StepDescription?.IndexOf(stepDescriptionFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (!string.IsNullOrEmpty(assignedToFilter))
                pendingSteps = pendingSteps.Where(x =>
                    (x.FullName != null && x.FullName.IndexOf(assignedToFilter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.GroupDescription != null && x.GroupDescription.IndexOf(assignedToFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out var parsedDate))
                pendingSteps = pendingSteps.Where(x => x.DateAssigned.Date == parsedDate.Date).ToList();

            // Rework Check (optional: keep or remove)
            using (var conn = GetConnection())
            {
                conn.Open();
                foreach (var step in pendingSteps)
                {
                    var cmd = new SqlCommand("SELECT COUNT(1) FROM foProcessEvents WHERE ProcessInstanceID = @ProcessInstanceID AND PreviousEventID < 0", conn);
                    cmd.Parameters.AddWithValue("@ProcessInstanceID", step.ProcessInstanceID);
                    step.IsReworkInstance = (int)cmd.ExecuteScalar() > 0;
                }
            }

            // Paging
            int totalRecords = pendingSteps.Count;
            ViewBag.totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.pageNumber = pageNumber;

            pendingSteps = pendingSteps
                .OrderByDescending(x => x.DateAssigned)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // User Name
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand("SELECT FirstName + ' ' + LastName FROM foUsers WHERE ID = @ID", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ID", userId);
                ViewBag.FullName = cmd.ExecuteScalar()?.ToString() ?? "";
            }

            ViewBag.UserID = userId;

            // Determine if any filters are applied
            ViewBag.SearchVisible = !string.IsNullOrEmpty(processnumberFilter) ||
                                    !string.IsNullOrEmpty(processNameFilter) ||
                                    !string.IsNullOrEmpty(stepDescriptionFilter) ||
                                    !string.IsNullOrEmpty(assignedToFilter) ||
                                    !string.IsNullOrEmpty(dateFilter);

            return View(pendingSteps);
        }



        private bool IsReworkInstance(SqlConnection conn, long processInstanceId)
        {
            using (var cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM foApprovalEvents 
        WHERE ProcessInstanceID = @ProcessInstanceID 
          AND PreviousEventID < 0", conn))
            {
                cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}
