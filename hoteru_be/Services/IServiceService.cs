using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services
{
    public interface IServiceService
    {
        Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery, CancellationToken ct = default);

        Task<MethodResultDTO<ServiceDTO>> GetSpecificService(int idService, CancellationToken ct);

        Task<MethodResultDTO> DeleteService(int idService, CancellationToken ct);

        Task<MethodResultDTO> PostService(ServiceDTO serviceDTO, CancellationToken ct);

        Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO, CancellationToken ct);
    }
}
