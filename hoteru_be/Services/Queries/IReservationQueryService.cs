using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IReservationQueryService
    {
        Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default);
        Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default);
        Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default);
        Task<MethodResultDTO<FullReservationDTO>> GetSpecificHistory(int idReservation, CancellationToken ct = default);
        Task<MethodResultDTO<ArrivalDTO>> GetSpecificArrival(int idArrival, CancellationToken ct = default);
    }
}
