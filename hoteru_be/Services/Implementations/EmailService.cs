using hoteru_be.Services.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService()
    {
        _smtpClient = new SmtpClient("smtp.example.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("username@example.com", "password"),
            EnableSsl = true,
        };
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("noreply@example.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        try
        {
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (SmtpException ex)
        {
            throw new InvalidOperationException("Failed to send email.", ex);
        }
    }
}
