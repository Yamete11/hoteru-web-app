using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message, CancellationToken ct);
    }
}
