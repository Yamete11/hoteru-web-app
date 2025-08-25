using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IAuthCommandService
    {
        Task<MethodResultDTO<AuthResponseDTO>> AuthenticateAsync(LoginDTO dto, CancellationToken ct = default);

        Task<MethodResultDTO<AuthResponseDTO>> RefreshAsync(string rawRefresh, string? ip, string? userAgent, CancellationToken ct = default);

        Task<MethodResultDTO> RevokeRefreshAsync(string rawRefresh, CancellationToken ct = default);
    }

}
