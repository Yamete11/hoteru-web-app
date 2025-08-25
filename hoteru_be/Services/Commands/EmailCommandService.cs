using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public class EmailCommandService : IEmailCommandService
    {
        private readonly IConfiguration _configuration;

        public EmailCommandService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message, CancellationToken ct)
        {
            var mail = _configuration["EmailSettings:Email"];
            var pw = _configuration["EmailSettings:Password"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            try
            {
                var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, pw)
                };

                var mailMessage = new MailMessage(mail, email, subject, message);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP error: {ex.Message}");
                Console.WriteLine($"Inner: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
