using System.Net.Mail;
using System.Net;

public class MailService
{
    private readonly IConfiguration _config;
    public MailService(IConfiguration config) => _config = config;

    public async Task SendEmailAsync(string toEmail, string subject, string body, byte[] attachment = null, string filename = "")
    {
        var message = new MailMessage
        {
            From = new MailAddress(_config["MailSettings:From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        if (attachment != null)
            message.Attachments.Add(new Attachment(new MemoryStream(attachment), filename));

        using var client = new SmtpClient(_config["MailSettings:SmtpServer"], int.Parse(_config["MailSettings:Port"]))
        {
            Credentials = new NetworkCredential(_config["MailSettings:Username"], _config["MailSettings:Password"]),
            EnableSsl = true
        };

        await client.SendMailAsync(message);
    }
}
