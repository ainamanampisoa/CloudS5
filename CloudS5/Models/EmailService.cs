using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using MailKit.Security; 

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
            // Utilisation de STARTTLS avec le port 587
            client.Connect(_emailSettings.SmtpHost, 465, SecureSocketOptions.SslOnConnect);

            // Authentification avec le nom d'utilisateur et mot de passe
            client.Authenticate(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

            // Envoi du message
            client.Send(emailMessage);

            // Déconnexion après l'envoi
            client.Disconnect(true);
        }
    }
}
