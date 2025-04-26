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

        public IActionResult PendingStep(int processId, int? stepId, int? processInstanceId, int userId)
        {
            SetUserAccess(userId);
            ViewBag.userid = userId;
            ViewBag.processInstanceId = processInstanceId;

            string stepDescription = "";
            double stepNo = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    // Get first step if not supplied
                    if (stepId is null)
                    {
                        var cmd = new SqlCommand(@"
                    SELECT TOP 1 ID 
                    FROM foApprovalSteps 
                    WHERE ProcessID = @ProcessID AND Active = 1 
                    ORDER BY StepNo", conn, transaction);
                        cmd.Parameters.AddWithValue("@ProcessID", processId);
                        var result = cmd.ExecuteScalar();
                        if (result == null)
                            return NotFound("No approval steps defined.");
                        stepId = Convert.ToInt32(result);
                    }

                    // Read step description + no
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

                    // ✅ Safe to load history now
                    PopulateProcessHistory(conn, transaction, processInstanceId);

                    transaction.Commit(); // Optional here, but safe
                }
            }

            ViewBag.StepDescription = stepDescription;
            ViewBag.StepNo = stepNo;
            ViewBag.stepId = stepId;
            ViewBag.processid = processId;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveApproval(IFormCollection form, int processid, int userId, int stepId, int processInstanceId, string stepDescription)
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
                    int approvalEventId;

                    var existingEventCmd = new SqlCommand(@"
                SELECT TOP 1 ID FROM foApprovalEvents 
                WHERE ProcessInstanceID = @ProcessInstanceID AND StepID = @StepID AND DateCompleted IS NULL 
                ORDER BY ID DESC", conn, transaction);
                    existingEventCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    existingEventCmd.Parameters.AddWithValue("@StepID", stepId);

                    var existingEventResult = existingEventCmd.ExecuteScalar();

                    if (existingEventResult != null)
                    {
                        approvalEventId = Convert.ToInt32(existingEventResult);
                        var updateEvent = new SqlCommand("UPDATE foApprovalEvents SET DateCompleted = GETDATE() WHERE ID = @ID", conn, transaction);
                        updateEvent.Parameters.AddWithValue("@ID", approvalEventId);
                        updateEvent.ExecuteNonQuery();
                    }
                    else
                    {
                        var insertEvent = new SqlCommand(@"
                    INSERT INTO foApprovalEvents (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, DateCompleted, Active)
                    OUTPUT INSERTED.ID
                    VALUES (@ProcessInstanceID, @StepID, 0, @UserID, GETDATE(), GETDATE(), 1)", conn, transaction);
                        insertEvent.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                        insertEvent.Parameters.AddWithValue("@StepID", stepId);
                        insertEvent.Parameters.AddWithValue("@UserID", userId);
                        approvalEventId = Convert.ToInt32(insertEvent.ExecuteScalar());
                    }

                    var insertApproval = new SqlCommand(@"
                INSERT INTO foApprovals (ProcessInstanceID, ApprovalEventID, StepID, Decision, Comment, CreatedDate, CreatedUserID, Active)
                OUTPUT INSERTED.ID
                VALUES (@ProcessInstanceID, @ApprovalEventID, @StepID, @Decision, @Comment, GETDATE(), @UserID, 1)", conn, transaction);
                    insertApproval.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    insertApproval.Parameters.AddWithValue("@ApprovalEventID", approvalEventId);
                    insertApproval.Parameters.AddWithValue("@StepID", stepId);
                    insertApproval.Parameters.AddWithValue("@Decision", decision);
                    insertApproval.Parameters.AddWithValue("@Comment", string.IsNullOrWhiteSpace(comment) ? DBNull.Value : comment);
                    insertApproval.Parameters.AddWithValue("@UserID", userId);

                    var approvalId = Convert.ToInt32(insertApproval.ExecuteScalar());

                    var auditData = new Dictionary<string, object>
            {
                { "StepDescription", stepDescription },
                { "Decision", decision },
                { "Comment", comment },
                { "CreatedUserID", userId },
                { "CreatedDate", DateTime.Now }
            };

                    var audit = new SqlCommand(@"
                INSERT INTO foApprovalEventsDetail (ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
                VALUES (@ApprovalEventID, @ProcessInstanceID, @StepID, @RecordID, @DataSetUpdate, GETDATE(), @CreatedUserID, 1)", conn, transaction);
                    audit.Parameters.AddWithValue("@ApprovalEventID", approvalEventId);
                    audit.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                    audit.Parameters.AddWithValue("@StepID", stepId);
                    audit.Parameters.AddWithValue("@RecordID", approvalId);
                    audit.Parameters.AddWithValue("@DataSetUpdate", JsonConvert.SerializeObject(auditData));
                    audit.Parameters.AddWithValue("@CreatedUserID", userId);
                    audit.ExecuteNonQuery();

                    foreach (var file in Request.Form.Files)
                    {
                        if (file == null || file.Length == 0) continue;

                        var descKey = $"desc_{file.Name}";
                        var description = form.TryGetValue(descKey, out var dVal) ? dVal.ToString() : "";

                        var fileName = Path.GetFileName(file.FileName);
                        var savePath = Path.Combine(basePath, fileName);

                        using (var stream = new FileStream(savePath, FileMode.Create)) file.CopyTo(stream);

                        var insertAttachment = new SqlCommand(@"
                    INSERT INTO foApprovalAttachments (ApprovalID, AttachmentDescription, AttachmentPath, CreatedDate, CreatedbyID, Active)
                    VALUES (@ApprovalID, @AttachmentDescription, @AttachmentPath, GETDATE(), @CreatedbyID, 1)", conn, transaction);
                        insertAttachment.Parameters.AddWithValue("@ApprovalID", approvalId);
                        insertAttachment.Parameters.AddWithValue("@AttachmentDescription", string.IsNullOrWhiteSpace(description) ? DBNull.Value : description);
                        insertAttachment.Parameters.AddWithValue("@AttachmentPath", Path.Combine("Attachments", "Approvals", fileName));
                        insertAttachment.Parameters.AddWithValue("@CreatedbyID", userId);
                        insertAttachment.ExecuteNonQuery();
                    }

                    string redirectAction = "PendingItems";
                    string redirectController = "ProcessEvents";

                    if (decision == "Approve")
                    {
                        var nextStepCmd = new SqlCommand(@"
        SELECT TOP 1 ID FROM foApprovalSteps 
        WHERE ProcessID = (SELECT ProcessID FROM foApprovalSteps WHERE ID = @StepID)
        AND StepNo > (SELECT StepNo FROM foApprovalSteps WHERE ID = @StepID)
        ORDER BY StepNo", conn, transaction);
                        nextStepCmd.Parameters.AddWithValue("@StepID", stepId);
                        var nextStep = nextStepCmd.ExecuteScalar();

                        if (nextStep != null)
                        {
                            var assignNext = new SqlCommand(@"
            INSERT INTO foApprovalEvents (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, Active)
            SELECT @ProcessInstanceID, ID, @PreviousEventID, GroupID, UserID, GETDATE(), 1
            FROM foApprovalSteps WHERE ID = @NextStepID", conn, transaction);
                            assignNext.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                            assignNext.Parameters.AddWithValue("@PreviousEventID", approvalEventId);
                            assignNext.Parameters.AddWithValue("@NextStepID", Convert.ToInt32(nextStep));
                            assignNext.ExecuteNonQuery();

                            redirectAction = "PendingApprovalItems";
                            redirectController = "ApprovalEvents";
                        }
                        else
                        {
                            ArchiveApproval(processInstanceId, conn, transaction);
                            redirectAction = "PendingApprovalItems";
                            redirectController = "ApprovalEvents";
                        }
                    }
                    else if (decision == "Decline")
                    {
                        ArchiveApproval(processInstanceId, conn, transaction);
                        redirectAction = "PendingItems";
                        redirectController = "ProcessEvents";
                    }
                    else if (decision == "Rework")
                    {
                        int processId = GetProcessIdForStep(stepId, conn, transaction);

                        // Get the first step in the process
                        var firstStepCmd = new SqlCommand(@"
        SELECT TOP 1 ID 
        FROM foProcessSteps 
        WHERE ProcessID = @ProcessID 
        ORDER BY StepNo", conn, transaction);
                        firstStepCmd.Parameters.AddWithValue("@ProcessID", processId);
                        var firstStepId = Convert.ToInt32(firstStepCmd.ExecuteScalar());

                        // 🔍 Find the most recent completed entry for the first step
                        var reworkUserCmd = new SqlCommand(@"
        SELECT TOP 1 UserID 
        FROM foProcessEvents 
        WHERE StepID = @StepID AND ProcessInstanceID = @ProcessInstanceID 
        ORDER BY ID DESC", conn, transaction);
                        reworkUserCmd.Parameters.AddWithValue("@StepID", firstStepId);
                        reworkUserCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                        var reworkUserId = Convert.ToInt32(reworkUserCmd.ExecuteScalar());

                        // ➕ Reassign the first step to that user
                        var reworkCmd = new SqlCommand(@"
        INSERT INTO foProcessEvents (ProcessInstanceID, StepID, PreviousEventID, UserID, DateAssigned, Active)
        VALUES (@ProcessInstanceID, @StepID, @PreviousEventID, @UserID, GETDATE(), 1)", conn, transaction);
                        reworkCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
                        reworkCmd.Parameters.AddWithValue("@PreviousEventID", -approvalEventId);
                        reworkCmd.Parameters.AddWithValue("@StepID", firstStepId);
                        reworkCmd.Parameters.AddWithValue("@UserID", reworkUserId); // ✅ now the correct original user
                        reworkCmd.ExecuteNonQuery();

                        redirectAction = "PendingItems";
                        redirectController = "ProcessEvents";
                    }


                    transaction.Commit();
                    return RedirectToAction(redirectAction, redirectController, new { userId });


                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Error"] = "Approval save failed: " + ex.Message;
                    return RedirectToAction("PendingStep", new { processid, stepId, processInstanceId, userId });
                }
            }
        }

        private void ArchiveApproval(int processInstanceId, SqlConnection conn, SqlTransaction transaction)
        {
            var archiveEvents = new SqlCommand(@"
        INSERT INTO foApprovalEventsArchive (ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active)
        SELECT ProcessInstanceID, StepID, PreviousEventID, GroupID, UserID, DateAssigned, DateCompleted, Active
        FROM foApprovalEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveEvents.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveEvents.ExecuteNonQuery();

            var archiveDetails = new SqlCommand(@"
        INSERT INTO foApprovalEventsDetailArchive (ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active)
        SELECT ApprovalEventID, ProcessInstanceID, StepID, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, Active
        FROM foApprovalEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            archiveDetails.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            archiveDetails.ExecuteNonQuery();

            var deleteEvents = new SqlCommand("DELETE FROM foApprovalEvents WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteEvents.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteEvents.ExecuteNonQuery();

            var deleteDetails = new SqlCommand("DELETE FROM foApprovalEventsDetail WHERE ProcessInstanceID = @ProcessInstanceID", conn, transaction);
            deleteDetails.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            deleteDetails.ExecuteNonQuery();
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

        public IActionResult PendingapprovalItems(int userId)
        {
            SetUserAccess(userId);
            var pendingSteps = new List<PendingStepViewModel>();

            using (var conn = GetConnection())
            {
                conn.Open();

                string query = @"
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
                s.StepNo,
                s.ProcessID
            FROM foApprovalEvents e
            JOIN foApprovalSteps s ON e.StepID = s.ID 
            LEFT JOIN foGroups g ON e.GroupID = g.ID
            LEFT JOIN foUsers u ON e.UserID = u.ID
            WHERE e.DateCompleted IS NULL
              AND (e.UserID = @UserID 
                   OR e.GroupID IN (SELECT GroupID FROM foUserGroups WHERE UserID = @UserID))
            ORDER BY e.DateAssigned DESC";

                using (var cmd = new SqlCommand(query, conn))
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
                                ProcessID = Convert.ToInt32(reader["ProcessID"])
                            });
                        }
                    }
                }
            }

            ViewBag.UserID = userId;
            return View(pendingSteps);
        }

        [HttpPost]
        public IActionResult ClaimStep(int eventId, int userId, int stepId, int processInstanceId, int processId)
        {
            using var conn = GetConnection();
            conn.Open();

            var cmd = new SqlCommand("UPDATE foApprovalEvents SET UserID = @UserID, GroupID = NULL WHERE ID = @EventID AND UserID IS NULL", conn);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@EventID", eventId);
            cmd.ExecuteNonQuery();

            return RedirectToAction("PendingStep", new { processId, stepId, processInstanceId, userId });
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
            string query = $"SELECT ID, Description FROM {tableName}";

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            options.Add(new SelectListItem
                            {
                                Value = reader["ID"].ToString(),
                                Text = reader["Description"].ToString()
                            });
                        }
                    }
                }
            }

            return options;
        }

        private void PopulateProcessHistory(SqlConnection conn, SqlTransaction transaction, int? processInstanceId)
        {
            if (!processInstanceId.HasValue) return;

            var history = new List<ProcessEventAuditViewModel>();

            // 🔹 PROCESS EVENTS
            var processCmd = new SqlCommand(@"
        SELECT ProcessEventID, StepID, TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo
FROM foProcessEventsDetail a LEFT OUTER JOIN foProcessSteps b on a.StepID = b.ID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            processCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            var processHistory = new List<ProcessEventAuditViewModel>();
            using (var reader = processCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                    processHistory.Add(new ProcessEventAuditViewModel
                    {
                        ProcessEventID = Convert.ToInt32(reader["ProcessEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        Data = data
                    });
                }
            }

            // 🔹 APPROVAL EVENTS
            var approvalCmd = new SqlCommand(@"
        SELECT ApprovalEventID, StepID, 'foApproval' AS TableName, RecordID, DataSetUpdate, CreatedDate, CreatedUserID, StepNo
FROM foApprovalEventsDetail a LEFT OUTER JOIN foApprovalSteps b on a.StepID = b.ID
        WHERE ProcessInstanceID = @ProcessInstanceID
        ORDER BY CreatedDate", conn, transaction);

            approvalCmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);

            var approvalHistory = new List<ProcessEventAuditViewModel>();
            using (var reader = approvalCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var json = reader["DataSetUpdate"].ToString();
                    var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    data["StepType"] = "Approval";

                    approvalHistory.Add(new ProcessEventAuditViewModel
                    {
                        ProcessEventID = Convert.ToInt32(reader["ApprovalEventID"]),
                        StepNo = Convert.ToDecimal(reader["StepNo"]),
                        TableName = reader["TableName"].ToString(),
                        RecordID = Convert.ToInt32(reader["RecordID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        CreatedUserID = Convert.ToInt32(reader["CreatedUserID"]),
                        Data = data
                    });
                }
            }

            // 🧩 Merge both types of events
            history = processHistory.Concat(approvalHistory).ToList();

            // 🔍 Resolve Foreign Keys + Attachments
            var foreignKeys = new Dictionary<string, List<ForeignKeyInfo>>();
            var foreignKeyOptions = new Dictionary<string, List<SelectListItem>>();
            var usedTables = history.Select(h => h.TableName).Distinct();

            foreach (var table in usedTables)
            {
                foreignKeys[table] = GetForeignKeyColumns(table)
                    .Where(fk => fk.TableName.Contains("_md_"))
                    .ToList();

                foreach (var fk in foreignKeys[table])
                {
                    if (!foreignKeyOptions.ContainsKey(fk.TableName))
                    {
                        foreignKeyOptions[fk.TableName] = GetForeignKeyOptions(fk.TableName);
                    }
                }
            }

            foreach (var record in history)
            {
                if (foreignKeys.TryGetValue(record.TableName, out var fks))
                {
                    foreach (var fk in fks)
                    {
                        if (record.Data.TryGetValue(fk.ColumnName, out var fkVal) &&
                            fkVal != null &&
                            int.TryParse(fkVal.ToString(), out var fkId))
                        {
                            var label = foreignKeyOptions[fk.TableName]
                                .FirstOrDefault(o => o.Value == fkId.ToString())?.Text;

                            if (!string.IsNullOrWhiteSpace(label))
                            {
                                record.Data[fk.ColumnName] = $"{label} (ID: {fkId})";
                            }
                        }
                    }
                }

                // 📎 Handle attachment fields
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
                            var filename = Path.GetFileName(path);
                            record.Data[key] = $"<a href='/Attachments/{path}' target='_blank'>{desc ?? filename}</a>";
                        }
                        else
                        {
                            record.Data[key] = desc;
                        }
                    }
                }
            }

            // ⏱ Sort entire history chronologically
            ViewBag.ProcessHistory = history.OrderBy(h => h.CreatedDate).ToList();
        }


    }


}


