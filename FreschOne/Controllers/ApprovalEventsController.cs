using FreschOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;


namespace FreschOne.Controllers
{
    public class ApprovalEventsController : BaseController
    {
        public ApprovalEventsController(DatabaseHelper dbHelper, IConfiguration configuration) : base(dbHelper, configuration) { }
        private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public IActionResult PendingStep(int processId, int EventID, int? stepId, int? processInstanceId, int userId)
        {
            SetUserAccess(userId);
            ViewBag.userid = userId;
            ViewBag.processInstanceId = processInstanceId;

            string stepDescription = "";
            double? stepNo = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    // adhoc
                    if (stepId == 0)
                    {
                        // Read step description + no
                        string stepQuery = "SELECT 'Adhoc approval ' + ProcessName AS StepDescription FROM foProcess where ID = " + processId;
                        using (var detailCmd = new SqlCommand(stepQuery, conn, transaction))
                        {
                            detailCmd.Parameters.AddWithValue("@StepID", stepId);
                            using (var reader = detailCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    stepDescription = reader["StepDescription"].ToString();
                                    stepNo = null;
                                }
                            } // ✅ Reader closed here
                        }
                    }
                    else
                    {
                        // Normal
                        string stepQuery = "SELECT StepDescription, StepNo FROM foApprovalSteps WHERE ID = @StepID";
                        using (var detailCmd = new SqlCommand(stepQuery, conn, transaction))
                        {
                            detailCmd.Parameters.AddWithValue("@StepID", stepId);
                            using (var reader = detailCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    stepDescription = reader["StepDescription"].ToString();
                                    stepNo = Convert.ToDouble(reader["StepNo"]);
                                }
                            } // ✅ Reader closed here
                        }
                    }                        

                    // ✅ Safe to load history now
                    PopulateProcessHistory(conn, transaction, processInstanceId);

                    transaction.Commit(); // Optional here, but safe
                }
            }

            ViewBag.StepDescription = stepDescription;
            ViewBag.StepNo = stepNo;
            ViewBag.stepId = stepId;
            ViewBag.processid = processId;
            ViewBag.EventID = EventID;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveApproval(IFormCollection form, int EventID, int processid, int userId, int stepId, int processInstanceId, string stepDescription)
        {
            var decision = form["Decision"].ToString();
            var comment = form["Comment"].ToString();
            var rowErrors = new List<string>();

            if ((decision == "Rework" || decision == "Decline") && string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError("Comment", "Comment is required for Rework or Decline.");
                return RedirectToAction("PendingStep", new { stepId, processInstanceId, userId });
            }

            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", "Approvals");
            Directory.CreateDirectory(basePath);

            using (var conn = GetConnection())
            {
                conn.Open();
                using var transaction = conn.BeginTransaction();

                try
                {
                    // ✅ 1. Mark the ApprovalEvent Completed
                    var updateApprovalEvent = new SqlCommand(@"
                UPDATE foApprovalEvents
                SET DateCompleted = GETDATE()
                WHERE ID = @ID", conn, transaction);
                    updateApprovalEvent.Parameters.AddWithValue("@ID", EventID);
                    updateApprovalEvent.ExecuteNonQuery();

                    // ✅ 2. Insert into foApprovals
                    var insertApproval = new SqlCommand(@"
                INSERT INTO foApprovals 
                (ProcessInstanceID, ApprovalEventID, StepID, Decision, Comment, CreatedDate, CreatedUserID, Active)
                OUTPUT INSERTED.ID
                VALUES (@ProcessInstanceID, @ApprovalEventID, @StepID, @Decision, @Comment, GETDATE(), @CreatedUserID, 1)", conn, transaction);
                    insertApproval.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertApproval.Parameters.AddWithValue("@ApprovalEventID", EventID);
                    insertApproval.Parameters.AddWithValue("@StepID", stepId);
                    insertApproval.Parameters.AddWithValue("@Decision", decision);
                    insertApproval.Parameters.AddWithValue("@Comment", string.IsNullOrWhiteSpace(comment) ? DBNull.Value : comment);
                    insertApproval.Parameters.AddWithValue("@CreatedUserID", userId);

                    int approvalId = Convert.ToInt32(insertApproval.ExecuteScalar());

                    // ✅ 3. Audit Insert
                    var auditData = new Dictionary<string, object>
            {
                { "StepDescription", stepDescription },
                { "Decision", decision },
                { "Comment", comment },
                { "CreatedUserID", userId },
                { "CreatedDate", DateTime.Now }
            };
                    var insertAudit = new SqlCommand(@"
                INSERT INTO foApprovalEventsDetail
                (ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
                VALUES (@ApprovalEventID, @ProcessInstanceID, @StepID, @RecordID, @DataSetUpdate, GETDATE(), @CreatedUserID, 1)", conn, transaction);
                    insertAudit.Parameters.AddWithValue("@ApprovalEventID", EventID);
                    insertAudit.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertAudit.Parameters.AddWithValue("@StepID", stepId);
                    insertAudit.Parameters.AddWithValue("@RecordID", approvalId);
                    insertAudit.Parameters.AddWithValue("@DataSetUpdate", JsonConvert.SerializeObject(auditData));
                    insertAudit.Parameters.AddWithValue("@CreatedUserID", userId);
                    insertAudit.ExecuteNonQuery();

                    // ✅ 4. Attachments
                    foreach (var file in Request.Form.Files)
                    {
                        if (file == null || file.Length == 0) continue;

                        var descKey = $"desc_{file.Name}";
                        var description = form.TryGetValue(descKey, out var descVal) ? descVal.ToString() : "";

                        var fileName = Path.GetFileName(file.FileName);
                        var savePath = Path.Combine(basePath, fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create))
                            file.CopyTo(stream);

                        var insertAttachment = new SqlCommand(@"
                    INSERT INTO foApprovalAttachments
                    (ApprovalID, AttachmentDescription, AttachmentPath, CreatedDate, CreatedbyID, Active)
                    VALUES (@ApprovalID, @AttachmentDescription, @AttachmentPath, GETDATE(), @CreatedbyID, 1)", conn, transaction);
                        insertAttachment.Parameters.AddWithValue("@ApprovalID", approvalId);
                        insertAttachment.Parameters.AddWithValue("@AttachmentDescription", string.IsNullOrWhiteSpace(description) ? DBNull.Value : description);
                        insertAttachment.Parameters.AddWithValue("@AttachmentPath", Path.Combine("Attachments", "Approvals", fileName));
                        insertAttachment.Parameters.AddWithValue("@CreatedbyID", userId);
                        insertAttachment.ExecuteNonQuery();
                    }

                    // ✅ 5. Fetch Approval Event Info
                    int? approvalStepId = null;
                    int? previousEventId = null;

                    using (var getEventCmd = new SqlCommand(@"
                    SELECT StepID, PreviousEventID
                    FROM foApprovalEvents
                    WHERE ID = @id", conn, transaction))
                    {
                        getEventCmd.Parameters.AddWithValue("@id", EventID);
                        using (var reader = getEventCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                approvalStepId = reader["StepID"] != DBNull.Value ? Convert.ToInt32(reader["StepID"]) : (int?)null; 
                                previousEventId = reader["PreviousEventID"] != DBNull.Value ? Convert.ToInt32(reader["PreviousEventID"]) : (int?)null;
                            }
                        }
                    }

                    // ✅ 6. Routing Based on Decision
                    if (decision == "Approve")
                    {
                        if (approvalStepId == 0)
                        {
                            // 🔥 Ad-hoc approval handling
                            int pendingCount;
                            using (var countCmd = new SqlCommand(@"
                        SELECT COUNT(*)
                        FROM foApprovalEvents
                        WHERE PreviousEventID = @PreviousEventID
                          AND DateCompleted IS NULL
                          AND Active = 1", conn, transaction))
                            {
                                countCmd.Parameters.AddWithValue("@PreviousEventID", previousEventId);
                                pendingCount = Convert.ToInt32(countCmd.ExecuteScalar());
                            }

                            if (pendingCount == 0)
                            {
                                ArchiveApprovalAndProcess(processInstanceId, conn, transaction);
                            }

                        }
                        else
                        {
                            // 🔥 Normal approval step: check next step
                            var nextStepCmd = new SqlCommand(@"
                        SELECT TOP 1 ID
                        FROM foApprovalSteps
                        WHERE ProcessID = (SELECT ProcessID FROM foApprovalSteps WHERE ID = @StepID)
                          AND StepNo > (SELECT StepNo FROM foApprovalSteps WHERE ID = @StepID)
                          AND Active = 1
                        ORDER BY StepNo", conn, transaction);
                            nextStepCmd.Parameters.AddWithValue("@StepID", approvalStepId.Value);

                            var nextStep = nextStepCmd.ExecuteScalar();

                            if (nextStep != null)
                            {
                                var assignNext = new SqlCommand(@"
                            INSERT INTO foApprovalEvents (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
                            SELECT @ProcessInstanceID, ID, @PreviousEventID, GroupID, UserID, GETDATE(), 1
                            FROM foApprovalSteps WHERE ID = @NextStepID", conn, transaction);
                                assignNext.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                assignNext.Parameters.AddWithValue("@PreviousEventID", EventID);
                                assignNext.Parameters.AddWithValue("@NextStepID", Convert.ToInt32(nextStep));
                                assignNext.ExecuteNonQuery();
                            }
                            else
                            {
                                ArchiveApprovalAndProcess(processInstanceId, conn, transaction);
                            }

                        }
                    }
                    else if (decision == "Decline")
                    {
                        ArchiveApprovalAndProcess(processInstanceId, conn, transaction);
                    }
                    else if (decision == "Rework")
                    {
                        if (approvalStepId == 0) ///if it is adhoc approval
                        {
                           // ✅ Close all other open ad-hoc approval events for this group
                            var closeOtherAdhoc = new SqlCommand(@"
                        UPDATE foApprovalEvents
                        SET DateCompleted = GETDATE()
                        WHERE ProcessInstanceID = @ProcessInstanceID
                          AND StepID = 0
                          AND ID <> @EventID
                          AND DateCompleted IS NULL
                          AND Active = 1", conn, transaction);

                            closeOtherAdhoc.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            closeOtherAdhoc.Parameters.AddWithValue("@EventID", EventID);
                            closeOtherAdhoc.ExecuteNonQuery();

                            int? reworkUserId = 0;
                            int? firstStepId = 0;

                            using (var getEventCmd = new SqlCommand(@"
                    SELECT TOP 1 ID, UserID, StepID
                    FROM foProcessEvents
                    WHERE ProcessInstanceID = @ProcessInstanceID 
                    ORDER BY ID DESC", conn, transaction))
                            {
                                getEventCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                                using (var reader = getEventCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        reworkUserId = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : (int?)null;
                                        previousEventId = reader["ID"] != DBNull.Value ? Convert.ToInt32(reader["ID"]) : (int?)null;
                                        firstStepId = reader["StepID"] != DBNull.Value ? Convert.ToInt32(reader["StepID"]) : (int?)null;
                                    }
                                }
                            }

                            var reworkCmd = new SqlCommand(@"
                    INSERT INTO foProcessEvents (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, Active)
                    VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @UserID, GETDATE(), 1)", conn, transaction);
                            reworkCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            reworkCmd.Parameters.AddWithValue("@PreviousEventID", -EventID); // 🔥 Negative ID for rework!
                            reworkCmd.Parameters.AddWithValue("@StepID", firstStepId);
                            reworkCmd.Parameters.AddWithValue("@UserID", reworkUserId);
                            reworkCmd.ExecuteNonQuery();

                            ViewBag.action = "Step send for Rework";
                            string nextAssignmentMessageRework = GetNextStepAssignment(conn, transaction, EventID, decision);

                            // ✅ Now safe to commit
                            transaction.Commit();

                            // Store message in TempData so it can survive redirect
                            TempData["SuccessMessage"] = nextAssignmentMessageRework;
                            TempData["UserId"] = userId;

                            return RedirectToAction("StepCompleted", "StepCompleted", new { message = nextAssignmentMessageRework, userId, actionheader = decision, processInstanceId = processInstanceId });

                        }
                        else
                        {
                            // 🔥 Rework the original process
                            int processIdFetched = GetProcessIdForStep(stepId, conn, transaction);

                            var firstStepCmd = new SqlCommand(@"
                    SELECT TOP 1 ID FROM foProcessSteps
                    WHERE ProcessID = @ProcessID
                    ORDER BY StepNo", conn, transaction);
                            firstStepCmd.Parameters.AddWithValue("@ProcessID", processIdFetched);

                            var firstStepId = Convert.ToInt32(firstStepCmd.ExecuteScalar());

                            var reworkUserCmd = new SqlCommand(@"
                    SELECT TOP 1 UserID
                    FROM foProcessEvents
                    WHERE StepID = @StepID AND ProcessInstanceID = @ProcessInstanceID
                    ORDER BY ID DESC", conn, transaction);
                            reworkUserCmd.Parameters.AddWithValue("@StepID", firstStepId);
                            reworkUserCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

                            var reworkUserId = Convert.ToInt32(reworkUserCmd.ExecuteScalar());

                            var reworkCmd = new SqlCommand(@"
                    INSERT INTO foProcessEvents (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, Active)
                    VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @UserID, GETDATE(), 1)", conn, transaction);
                            reworkCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            reworkCmd.Parameters.AddWithValue("@PreviousEventID", -EventID); // 🔥 Negative ID for rework!
                            reworkCmd.Parameters.AddWithValue("@StepID", firstStepId);
                            reworkCmd.Parameters.AddWithValue("@UserID", reworkUserId);
                            reworkCmd.ExecuteNonQuery();


                        }
                    }

                    string nextAssignmentMessage = GetNextStepAssignment(conn, transaction, EventID, decision);

                    // ✅ Now safe to commit
                    transaction.Commit();

                    // Store message in TempData so it can survive redirect
                    TempData["SuccessMessage"] = nextAssignmentMessage;
                    TempData["UserId"] = userId;

                    if (decision == "Rework")
                    {
                        ViewBag.action = "Step send for Rework";
                    }
                    else if (decision == "Approve")
                    {
                        ViewBag.action = "Step approved";
                    }
                    else if (decision == "Decline")
                    {
                        ViewBag.action = "Step declined";
                    }

                    return RedirectToAction("StepCompleted", "StepCompleted", new { message = nextAssignmentMessage, userId, actionheader = decision, processInstanceId = processInstanceId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Approval save failed: " + ex.Message;
                    return RedirectToAction("PendingStep", new { processid, stepId, processInstanceId, userId });
                }
            }
        }

        private void ArchiveApprovalAndProcess(int processInstanceId,SqlConnection conn, SqlTransaction transaction)
        {
            // 🟰 Copy Process Events into Archive
            var archiveEventsCmd = new SqlCommand(@"
        INSERT INTO foProcessEventsArchive (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active, Cancelled)
        SELECT ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active, Cancelled
        FROM foProcessEvents
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveEventsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveEventsCmd.ExecuteNonQuery();

            // 🟰 Copy Process Event Details into Archive
            var archiveDetailsCmd = new SqlCommand(@"
        INSERT INTO foProcessEventsDetailArchive (ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
        SELECT ProcessEventID, ProcessInstanceID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active
        FROM foProcessEventsDetail
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveDetailsCmd.ExecuteNonQuery();

            // 🟰 Copy Approval Events into Archive
            var archiveApprovalCmd = new SqlCommand(@"
        INSERT INTO foApprovalEventsArchive (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active)
        SELECT ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active
        FROM foApprovalEvents
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveApprovalCmd.ExecuteNonQuery();

            // 🟰 Copy Approval Event Details into Archive
            var archiveApprovalDetailsCmd = new SqlCommand(@"
        INSERT INTO foApprovalEventsDetailArchive (ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
        SELECT ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active
        FROM foApprovalEventsDetail
        WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveApprovalDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveApprovalDetailsCmd.ExecuteNonQuery();

            // 🔥 Delete or deactivate process events
            var deleteProcEventsCmd = new SqlCommand("DELETE FROM foProcessEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteProcEventsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteProcEventsCmd.ExecuteNonQuery();

            var deleteProcDetailsCmd = new SqlCommand("DELETE FROM foProcessEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteProcDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteProcDetailsCmd.ExecuteNonQuery();

            // 🔥 Delete or deactivate approval events
            var deleteApprovalCmd = new SqlCommand("DELETE FROM foApprovalEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteApprovalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteApprovalCmd.ExecuteNonQuery();

            var deleteApprovalDetailsCmd = new SqlCommand("DELETE FROM foApprovalEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteApprovalDetailsCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteApprovalDetailsCmd.ExecuteNonQuery();
        }

        private int GetProcessIdForStep(int stepId, SqlConnection conn, SqlTransaction tx)
        {
            var cmd = new SqlCommand("SELECT ProcessID FROM foApprovalSteps WHERE ID = @StepID", conn, tx);
            cmd.Parameters.AddWithValue("@StepID", stepId);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public IActionResult StepCompleted(int userId, int stepId, int processInstanceId)
        {
            ViewBag.userid = userId;
            ViewBag.stepId = stepId;
            ViewBag.processInstanceId = processInstanceId;
            ViewBag.Message = "Approval step completed.";
            return View();
        }


        [HttpPost]
        public IActionResult ClaimStep(int userId, int EventID, int stepId, int processInstanceId, int processId)
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand("UPDATE foApprovalEvents SET UserID = @UserID, GroupID = NULL WHERE ID = @id AND UserID IS NULL", conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@id", EventID);
            cmd.ExecuteNonQuery();

            return RedirectToAction("PendingStep", new { processId, EventID, stepId, processInstanceId, userId });

        }

        private List<ForeignKeyInfo> GetForeignKeyColumns(string tablename)
        {


            var foreignKeys = new List<ForeignKeyInfo>();
            string query = @"
        SELECT 
            c.name AS ColumnName,
            ref_tab.name AS ReferencedTableName,
            CASE WHEN RIGHT(c.name, 2) = 'ID' THEN LEFT(c.name, LEN(c.name) - 2) ELSE c.name END AS ColumnDescription
        FROM 
            sys.foreign_key_columns AS fkc
        INNER JOIN 
            sys.columns AS c ON fkc.parent_object_id = c.object_id 
            AND fkc.parent_column_id = c.column_id
        INNER JOIN 
            sys.tables AS parent_tab ON parent_tab.object_id = fkc.parent_object_id
        INNER JOIN 
            sys.tables AS ref_tab ON ref_tab.object_id = fkc.referenced_object_id
        WHERE 
            parent_tab.name = @TableName AND c.name LIKE '%ID'";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tablename);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreignKeys.Add(new ForeignKeyInfo
                            {
                                ColumnName = reader.GetString(0),
                                TableName = reader.GetString(1),
                                ColumnDescription = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return foreignKeys;
        }

        private List<SelectListItem> GetForeignKeyOptions(string tableName)
        {
            var options = new List<SelectListItem>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var ignoredColumns = GetIgnoredColumns(connection,null);
                var ignoredSet = new HashSet<string>(ignoredColumns, StringComparer.OrdinalIgnoreCase);

                var fkColumns = GetForeignKeyColumns(tableName).ToList();
                var fkLookup = fkColumns.ToDictionary(fk => fk.ColumnName, fk => fk.TableName, StringComparer.OrdinalIgnoreCase);

                // Get available columns for display
                var columnsQuery = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";
                var displayColumns = new List<string>();
                using (var cmd = new SqlCommand(columnsQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = reader["COLUMN_NAME"].ToString();
                            if (!col.Equals("ID", StringComparison.OrdinalIgnoreCase) && !ignoredSet.Contains(col))
                            {
                                displayColumns.Add(col);
                            }
                        }
                    }
                }

                if (displayColumns.Count == 0)
                    displayColumns.Add("ID"); // Fallback

                // Check if "Active" column exists
                bool hasActive = displayColumns.Any(c => c.Equals("Active", StringComparison.OrdinalIgnoreCase));
                var selectColumns = "ID, " + string.Join(", ", displayColumns);
                var query = hasActive
                    ? $"SELECT {selectColumns} FROM {tableName} WHERE Active = 1"
                    : $"SELECT {selectColumns} FROM {tableName}";

                using (var cmd = new SqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var displayParts = new List<string>();
                        foreach (var col in displayColumns)
                        {
                            var val = reader[col]?.ToString();
                            if (!string.IsNullOrEmpty(val) && fkLookup.ContainsKey(col))
                            {
                                var fkTable = fkLookup[col];
                                var fkOptions = GetForeignKeyOptions(fkTable);
                                var match = fkOptions.FirstOrDefault(x => x.Value == val);
                                if (match != null)
                                    val = match.Text;
                            }

                            displayParts.Add(val);
                        }

                        options.Add(new SelectListItem
                        {
                            Value = reader["ID"].ToString(),
                            Text = string.Join(" | ", displayParts)
                        });
                    }
                }
            }

            return options;
        }


        private void PopulateProcessHistory(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            var ignoredColumns = GetIgnoredColumns(conn, transaction);
            if (!processInstanceId.HasValue) return;

            var history = new List<ProcessEventAuditViewModel>();
            var approvalHistory = new List<ProcessEventAuditViewModel>();

            // 🟦 PROCESS EVENTS
            var cmd = new SqlCommand(@"
        SELECT a.id AS DetailID, ProcessEventID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, ISNULL(StepNo,1.00) AS StepNo,
               c.FirstName + ' ' + c.LastName AS FullName
        FROM foProcessEventsDetail a
        LEFT JOIN foProcessSteps b ON a.StepID = b.ID
        LEFT JOIN foUsers c ON c.ID = a.CreatedUserID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);
            cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = rawData
                        .Where(kvp => !ignoredColumns.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    history.Add(new ProcessEventAuditViewModel
                    {
                        DetailID = Convert.ToInt32(reader["DetailID"]),
                        ProcessEventID = Convert.ToInt32(reader["ProcessEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        FullName = reader["FullName"].ToString(),
                        Data = data
                    });
                }
            }

            // 🟨 APPROVAL EVENTS
            var approvalCmd = new SqlCommand(@"
        SELECT a.id AS DetailID, ApprovalEventID, StepID, 'foApproval' AS TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, ISNULL(StepNo,1.00) AS StepNo,
               c.FirstName + ' ' + c.LastName AS FullName
        FROM foApprovalEventsDetail a
        LEFT JOIN foApprovalSteps b ON a.StepID = b.ID
        LEFT JOIN foUsers c ON c.ID = a.CreatedUserID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);
            approvalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            using (var reader = approvalCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    var data = rawData
                        .Where(kvp => !ignoredColumns.Contains(kvp.Key))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    data["StepType"] = "Approval";

                    approvalHistory.Add(new ProcessEventAuditViewModel
                    {
                        DetailID = Convert.ToInt32(reader["DetailID"]),
                        ProcessEventID = Convert.ToInt32(reader["ApprovalEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        FullName = reader["FullName"].ToString(),
                        Data = data
                    });
                }
            }

            // 🧠 Resolve foreign key values for all tables involved
            var allRecords = history.Concat(approvalHistory).ToList();
            var usedTables = allRecords.Select(h => h.TableName).Distinct();

            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            foreach (var table in usedTables)
            {
                foreignKeys[table] = GetForeignKeyColumns(table);
                foreach (var fk in foreignKeys[table])
                {
                    if (!foreignKeyOptions.ContainsKey(fk.TableName))
                    {
                        foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                    }
                }
            }

            // 🔁 foUser lookup
            var userLookup = new Dictionary<string, string>();
            using (var cmd2 = new SqlCommand("SELECT ID, FirstName + ' ' + LastName AS FullName FROM foUsers", conn, transaction))
            using (var reader = cmd2.ExecuteReader())
            {
                while (reader.Read())
                {
                    userLookup[reader["ID"].ToString()] = reader["FullName"].ToString();
                }
            }

            // ✨ Resolve FK values and attachments
            foreach (var record in allRecords)
            {
                var table = record.TableName;

                // FK columns
                if (foreignKeys.TryGetValue(table, out var fks))
                {
                    foreach (var fk in fks)
                    {
                        if (record.Data.TryGetValue(fk.ColumnName, out var val) &&
                            val != null && int.TryParse(val.ToString(), out var fkId))
                        {
                            var label = foreignKeyOptions[fk.TableName]
                                .FirstOrDefault(o => o.Value == fkId.ToString())?.Text;

                            if (!string.IsNullOrWhiteSpace(label))
                            {
                                record.Data[fk.ColumnName] = label;
                            }
                        }
                    }
                }

                // foUserID_* fields
                foreach (var kvp in record.Data.ToList())
                {
                    if (kvp.Key.StartsWith("foUserID_", StringComparison.OrdinalIgnoreCase))
                    {
                        var userId = kvp.Value?.ToString();
                        if (!string.IsNullOrEmpty(userId) && userLookup.TryGetValue(userId, out var name))
                        {
                            var newKey = kvp.Key.Replace("foUserID_", "").Replace("_", " ").Trim();
                            record.Data[newKey] = name;
                        }
                        record.Data.Remove(kvp.Key);
                    }
                }

                // attachment_ fields
                foreach (var key in record.Data.Keys.Where(k => k.StartsWith("attachment_")).ToList())
                {
                    var val = record.Data[key]?.ToString();
                    if (!string.IsNullOrEmpty(val) && val.Contains(';'))
                    {
                        var parts = val.Split(';');
                        var desc = parts[0];
                        var path = parts.Length > 1 ? parts[1] : "";
                        if (!string.IsNullOrEmpty(path))
                        {
                            var filename = System.IO.Path.GetFileName(path);
                            record.Data[key] = $"<a href='/Attachments/{path}' target='_blank'>{desc ?? filename}</a>";
                        }
                        else
                        {
                            record.Data[key] = desc;
                        }
                    }
                }
            }

            // ✅ Final merge
            ViewBag.ProcessHistory = allRecords;
        }

        private List<string> GetIgnoredColumns(SqlConnection conn, SqlTransaction transaction)
        {
            var ignoredColumns = new List<string>();
            string query = "SELECT ColumnName FROM foTableColumnsToIgnore";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ignoredColumns.Add(reader["ColumnName"].ToString());
                    }
                }
            }

            return ignoredColumns;
        }

        private string GetNextStepAssignment(SqlConnection conn, SqlTransaction transaction, int currentEventId, string decision)
        {

            var assignees = new List<string>();
            string stepDescription = null;
            DateTime? dateCompleted = null;

            // 🔍 Fetch timestamp from current event
            using (var tsCmd = new SqlCommand(@"SELECT DateCompleted FROM foApprovalEvents WHERE ID = @ID", conn, transaction))
            {
                tsCmd.Parameters.AddWithValue("@ID", currentEventId);
                var result = tsCmd.ExecuteScalar();
                if (result != DBNull.Value && result != null)
                    dateCompleted = Convert.ToDateTime(result);
            }

            // 1️⃣ Process Steps
            using (var processCmd = new SqlCommand(@"
        SELECT pe.UserID, u.FirstName, u.LastName, 
               pe.GroupID, g.Description AS GroupName, 
               s.StepDescription,
               pe.DateCompleted
        FROM foProcessEvents pe
        LEFT JOIN foUsers u ON pe.UserID = u.ID
        LEFT JOIN foGroups g ON pe.GroupID = g.ID
        LEFT JOIN foProcessSteps s ON s.ID = pe.StepID
        WHERE pe.PreviousEventID = @EventID", conn, transaction))
            {
                processCmd.Parameters.AddWithValue("@EventID", -currentEventId);
                using var reader = processCmd.ExecuteReader();
                while (reader.Read())
                {   
                    stepDescription ??= reader["StepDescription"]?.ToString() ?? "(Unnamed Step)";
                    dateCompleted ??= reader["DateCompleted"] != DBNull.Value ? Convert.ToDateTime(reader["DateCompleted"]) : (DateTime?)null;

                    if (reader["UserID"] != DBNull.Value)
                        assignees.Add($"👤 {reader["FirstName"]} {reader["LastName"]}");
                    else if (reader["GroupID"] != DBNull.Value)
                        assignees.Add($"🧑‍🤝‍🧑 {reader["GroupName"]}");
                }
            }

            // 2️⃣ Approval Steps
            using (var approvalCmd = new SqlCommand(@"
        SELECT ae.UserID, u.FirstName, u.LastName,
               ae.GroupID, g.Description AS GroupName,
               s.StepDescription,
               ae.DateCompleted
        FROM foApprovalEvents ae
        LEFT JOIN foUsers u ON ae.UserID = u.ID
        LEFT JOIN foGroups g ON ae.GroupID = g.ID
        LEFT JOIN foApprovalSteps s ON s.ID = ae.StepID
        WHERE ae.PreviousEventID = @EventID", conn, transaction))
            {
                approvalCmd.Parameters.AddWithValue("@EventID", currentEventId);
                using var reader = approvalCmd.ExecuteReader();
                while (reader.Read())
                {
                    stepDescription ??= reader["StepDescription"]?.ToString() ?? "(Unnamed Approval Step)";
                    dateCompleted ??= reader["DateCompleted"] != DBNull.Value ? Convert.ToDateTime(reader["DateCompleted"]) : (DateTime?)null;

                    if (reader["UserID"] != DBNull.Value)
                        assignees.Add($"👤 {reader["FirstName"]} {reader["LastName"]}");
                    else if (reader["GroupID"] != DBNull.Value)
                        assignees.Add($"🧑‍🤝‍🧑 {reader["GroupName"]}");
                }
            }

            // ✅ Final Footer with Timestamp + Event ID
            string timestamp = dateCompleted.HasValue
                ? $"<div><small class='text-muted'>Submitted on {dateCompleted.Value:yyyy-MM-dd HH:mm:ss}</small></div>"
                : "";
            string footer = $@"<hr>{timestamp}
                       <div><small class='text-muted'>ID: {currentEventId}</small></div>";

            // ✅ Final Message
            if (assignees.Any())
            {
                string assigneeList = string.Join("<br>- ", assignees);
                return $@"✅ Your {decision} step has been submitted.<br>
                  The next step is <strong>{stepDescription}</strong><br>
                  Assigned to:<br> {assigneeList}
                  {footer}";
            }
            else
            {
                return $@"✅ Your {decision} has been submitted.<br>
                  Process has been completed.
                  {footer}";
            }
        }

    }


}


