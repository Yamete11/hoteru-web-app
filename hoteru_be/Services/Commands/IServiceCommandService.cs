using hoteru_be.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public interface IServiceCommandService
    {
        Task<MethodResultDTO> PostService(ServiceDTO serviceDTO, CancellationToken ct);
        Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO, CancellationToken ct);
        Task<MethodResultDTO> DeleteService(int idService, CancellationToken ct);
    }
}
