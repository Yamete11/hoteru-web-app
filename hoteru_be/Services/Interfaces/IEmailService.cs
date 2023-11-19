using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
