using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public void SendEmail(string recipientEmail, string subject, string htmlContent)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        emailMessage.To.Add(new MailboxAddress("", recipientEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlContent
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            client.Connect(_emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.SmtpSsl);
            client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
            client.Send(emailMessage);
            client.Disconnect(true);
        }
    }
}
