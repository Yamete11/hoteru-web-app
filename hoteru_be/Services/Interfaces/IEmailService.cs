using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body);
    }
}
