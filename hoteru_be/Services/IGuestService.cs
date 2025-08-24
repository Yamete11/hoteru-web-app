using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services
{
    public interface IGuestService
    {
        Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string? searchQuery = null, string? searchField = null, CancellationToken ct = default);

        Task<MethodResultDTO<SpecificGuestDTO>> GetSpecificGuest(int idPerson, CancellationToken ct = default);

        Task<MethodResultDTO> DeleteGuest(int idPerson, CancellationToken ct = default);

        Task<MethodResultDTO> UpdateGuest(GuestDTO guestDTO, CancellationToken ct = default);

        Task<MethodResultDTO> PostGuest(GuestDTO guestDTO, CancellationToken ct = default);
    }
}
