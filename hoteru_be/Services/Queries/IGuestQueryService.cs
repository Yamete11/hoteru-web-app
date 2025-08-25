using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IGuestQueryService
    {
        Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string? searchQuery = null, string? searchField = null, CancellationToken ct = default);

        Task<MethodResultDTO<SpecificGuestDTO>> GetSpecificGuest(int idPerson, CancellationToken ct = default);
    }
}
