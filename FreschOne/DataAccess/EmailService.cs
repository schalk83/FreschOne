using FreschOne.Helpers;
using Microsoft.Data.SqlClient;

namespace FreschOne.DataAccess
{
    public class EmailService
    {
        public void QueueEmailNotification(SqlConnection conn, SqlTransaction transaction,
      long processInstanceId, long stepId, long groupId, long userId, string templateType)
        {
            var emailTemplateId = GetTemplateIdByType(conn, transaction, templateType);

            var cmd = new SqlCommand(@"
        INSERT INTO foEmailNotifications 
        (ProcessInstanceID, StepID, GroupID, UserID, EmailTemplateID, DateSent, Active)
        VALUES (@ProcessInstanceID, @StepID, @GroupID, @UserID, @EmailTemplateID, NULL, 1)", conn, transaction);

            cmd.Parameters.AddWithValue("@ProcessInstanceID", processInstanceId);
            cmd.Parameters.AddWithValue("@StepID", stepId);
            cmd.Parameters.AddWithValue("@GroupID", groupId);
            cmd.Parameters.AddWithValue("@UserID", userId);
            cmd.Parameters.AddWithValue("@EmailTemplateID", emailTemplateId);

            cmd.ExecuteNonQuery();
        }

        public void ProcessPendingEmails(SqlConnection conn, SqlTransaction transaction, string templateType)
        {
            // Optionally get Template ID by type
            var templateId = GetTemplateIdByType(conn, transaction, templateType);
            if (templateId == null)
            {
                Console.WriteLine("⚠️ No email template found for "+ templateType);
                return;
            }

            var cmd = new SqlCommand(@"
    SELECT n.ID, n.UserID, n.ProcessInstanceID, t.EmailSubject, t.EmailBody
    FROM foEmailNotifications n
    JOIN foEmailTemplate t ON t.ID = n.EmailTemplateID
    WHERE n.DateSent IS NULL AND n.Active = 1
    AND t.ID = @TemplateID", conn, transaction);

            cmd.Parameters.AddWithValue("@TemplateID", templateId.Value);

            var reader = cmd.ExecuteReader();

            var emailsToSend = new List<(int ID, long UserID, string Subject, string Body, long ProcessID)>();

            while (reader.Read())
            {
                emailsToSend.Add((
                    (int)reader["ID"],
                    (long)reader["UserID"],
                    reader["EmailSubject"]!.ToString(),     // Use ! if you're sure it's not null
                    reader["EmailBody"]!.ToString(),
                    (long)reader["ProcessInstanceID"]
                ));
            }

            reader.Close();

            foreach (var email in emailsToSend)
            {
                // Placeholder replacement logic
                string subject = email.Subject.Replace("{{ProcessID}}", email.ProcessID.ToString());
                string body = email.Body.Replace("{{ProcessID}}", email.ProcessID.ToString())
                                        .Replace("{{Description}}", "Demo Description"); // Pull from DB if needed

                string fullHtml = LoadEmailHtmlTemplate(subject, body);

                bool sent = SendEmail.SendEmailToUser("freedom.nxumalo@digioutsource.com", subject, fullHtml);
                if (sent)
                {
                    var updateCmd = new SqlCommand(
                  "UPDATE foEmailNotifications SET DateSent = GETDATE(), Active = 0 WHERE ID = @ID", conn);
                    updateCmd.Parameters.AddWithValue("@ID", email.ID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // optionally log failed attempt
                }

             
            }
        }


        public long? GetTemplateIdByType(SqlConnection conn, SqlTransaction transaction, string templateType)
        {
            var cmd = new SqlCommand(@"
        SELECT TOP 1 ID FROM dbo.foEmailTemplate
        WHERE TemplateType = @TemplateType", conn, transaction);

            cmd.Parameters.AddWithValue("@TemplateType", templateType ?? (object)DBNull.Value);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt64(result) : (long?)null;
        }
        public string LoadEmailHtmlTemplate(string subject, string body)
        {
           // var templatePath = Path.Combine(AppContext.BaseDirectory, "HtmlTemplates", "Emails.html");

            var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
            var templatePath = Path.Combine(projectRoot, "HtmlTemplates", "Emails.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Email template not found.", templatePath);

            string html = File.ReadAllText(templatePath);

            html = html.Replace("{{subject}}", subject)
                       .Replace("{{body}}", body);

            return html;
        }
    }
}
