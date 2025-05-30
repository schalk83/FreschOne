using DocumentFormat.OpenXml.Wordprocessing;
using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FreschOne.Controllers
{
    public class ArchivedItemsController : BaseController
    {
        public ArchivedItemsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult ArchivedItems(int userId, int pageNumber = 1, int pageSize = 10, string processnumberFilter = "", string processNameFilter = "", string stepNoFilter = "", string stepDescriptionFilter = "", string assignedToFilter = "", string dateFilter = "")
        {
            SetUserAccess(userId);
            var archivedSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                // Fetch ALL Archived Process Steps
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
                s.ProcessID,
                p.ProcessName
            FROM foProcessEventsArchive e
            LEFT JOIN foProcessSteps s ON e.StepID = s.ID
            LEFT JOIN foProcess p ON s.ProcessID = p.ID
            LEFT JOIN foGroups g ON e.GroupID = g.ID
            LEFT JOIN foUsers u ON e.UserID = u.ID
            WHERE (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
              AND e.ID = (SELECT TOP 1 ID FROM foProcessEventsArchive WHERE ProcessInstanceID = e.ProcessInstanceID ORDER BY StepNo DESC, DateAssigned DESC)";

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
                                StepID = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7) ? null : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                StepNo = reader.IsDBNull(10) ? 0 : Convert.ToDouble(reader["StepNo"]),
                                ProcessID = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader["ProcessID"]),
                                ProcessName = reader.IsDBNull(12) ? "" : reader.GetString(12),
                                StepType = "Process"
                            });
                        }
                    }
                }

                // Fetch ALL Archived Approval Steps
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
                s.ProcessID,
                p.ProcessName
            FROM foApprovalEventsArchive e
            LEFT JOIN foApprovalSteps s ON e.StepID = s.ID
            LEFT JOIN foProcess p ON s.ProcessID = p.ID
            LEFT JOIN foGroups g ON e.GroupID = g.ID
            LEFT JOIN foUsers u ON e.UserID = u.ID
            WHERE (e.UserID = @UserID OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
              AND e.ID = (SELECT TOP 1 ID FROM foApprovalEventsArchive WHERE ProcessInstanceID = e.ProcessInstanceID ORDER BY StepNo DESC, DateAssigned DESC)";

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
                                StepID = reader.IsDBNull(2) ? 0 : reader.GetInt64(2),
                                GroupID = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                                GroupDescription = reader.IsDBNull(4) ? null : reader.GetString(4),
                                UserID = reader.IsDBNull(5) ? (long?)null : reader.GetInt64(5),
                                FullName = reader.IsDBNull(6) && reader.IsDBNull(7) ? null : $"{reader.GetString(6)} {reader.GetString(7)}",
                                DateAssigned = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                StepDescription = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                StepNo = reader.IsDBNull(10) ? 0 : Convert.ToDouble(reader["StepNo"]),
                                ProcessID = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader["ProcessID"]),
                                ProcessName = reader.IsDBNull(12) ? "" : reader.GetString(12),
                                StepType = "Approval"
                            });
                        }
                    }
                }
            }

            // Apply search filters
            if (!string.IsNullOrEmpty(processnumberFilter))
                archivedSteps = archivedSteps.Where(x => x.ProcessInstanceID.ToString().Contains(processnumberFilter)).ToList();
            if (!string.IsNullOrEmpty(processNameFilter))
                archivedSteps = archivedSteps.Where(x => x.ProcessName?.IndexOf(processNameFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (!string.IsNullOrEmpty(stepNoFilter))
                archivedSteps = archivedSteps.Where(x => x.StepNo.ToString().Contains(stepNoFilter)).ToList();
            if (!string.IsNullOrEmpty(stepDescriptionFilter))
                archivedSteps = archivedSteps.Where(x => x.StepDescription?.IndexOf(stepDescriptionFilter, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            if (!string.IsNullOrEmpty(assignedToFilter))
                archivedSteps = archivedSteps.Where(x =>
                    (x.FullName != null && x.FullName.IndexOf(assignedToFilter, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (x.GroupDescription != null && x.GroupDescription.IndexOf(assignedToFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            if (!string.IsNullOrEmpty(dateFilter) && DateTime.TryParse(dateFilter, out var parsedDate))
                archivedSteps = archivedSteps.Where(x => x.DateAssigned.Date == parsedDate.Date).ToList();

            // Pagination
            int totalRecords = archivedSteps.Count;
            ViewBag.totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.pageNumber = pageNumber;

            archivedSteps = archivedSteps
                .OrderByDescending(x => x.DateAssigned)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // User's Name
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


            return View(archivedSteps);
        }

    }
}
