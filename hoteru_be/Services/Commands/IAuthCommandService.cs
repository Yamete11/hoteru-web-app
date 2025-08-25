using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IAuthCommandService
    {
        Task<MethodResultDTO<AuthResponseDTO>> AuthenticateAsync(LoginDTO dto, CancellationToken ct = default);
    }
}
