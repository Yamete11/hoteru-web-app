using hoteru_be.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class EmailService : IEmailService
    {

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "vasya.developer11@outlook.com";
            var pw = "Urahara1!";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                to: email,
                subject, message));

        }
    }
}
