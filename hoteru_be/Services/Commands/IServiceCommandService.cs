using hoteru_be.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public interface IServiceCommandService
    {
        Task<MethodResultDTO> PostService(int hotelId, ServiceDTO serviceDTO, CancellationToken ct);
        Task<MethodResultDTO> UpdateService(int hotelId, ServiceDTO serviceDTO, CancellationToken ct);
        Task<MethodResultDTO> DeleteService(int hotelId, int idService, CancellationToken ct);
    }
}
