using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<EmailCommandService> _logger;

        public EmailCommandService(IConfiguration configuration, ILogger<EmailCommandService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message, CancellationToken ct)
        {
            var mail = _configuration["EmailSettings:Email"];
            var pw = _configuration["EmailSettings:Password"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            try
            {
                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mail, pw)
                };

                var mailMessage = new MailMessage(mail, email, subject, message);
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("Email sent to {Email} with subject {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} with subject {Subject}", email, subject);
                throw;
            }
        }
    }
}
