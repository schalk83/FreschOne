using System.Net.Mail;
using System.Net;

namespace FreschOne.DataAccess
{
    public static class SendEmail
    {
        public static bool SendEmailToUser(string toAddress, string subject, string htmlBody)
        {
            var fromAddress = ""; 
            var smtpHost = "";          
            var smtpPort = 0;                         
            var smtpUser = "";            
            var smtpPass = "";

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(fromAddress);
                message.To.Add(new MailAddress(toAddress));
                message.Subject = subject;
                message.Body = htmlBody;
                message.IsBodyHtml = true;

                using var smtp = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                smtp.Send(message);
                Console.WriteLine($"✅ Email sent to {toAddress}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send email to {toAddress}: {ex.Message}");
                return false;
            }
        }
    }
}
