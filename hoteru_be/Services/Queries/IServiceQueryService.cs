using hoteru_be.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Queries
{
    public interface IServiceQueryService
    {
        Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery, CancellationToken ct = default);
        Task<MethodResultDTO<ServiceDTO>> GetSpecificService(int idService, CancellationToken ct);
    }
}
