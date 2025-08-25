using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public interface IEmailCommandService
    {
        Task SendEmailAsync(string email, string subject, string message, CancellationToken ct);
    }
}
