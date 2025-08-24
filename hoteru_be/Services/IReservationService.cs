using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services
{
    public interface IReservationService
    {
        Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit ,string searchQuery = "", string searchField = "", CancellationToken ct = default);

        Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default);

        Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default);

        Task<MethodResultDTO<FullReservationDTO>> GetSpecificHistory(int idReservation, CancellationToken ct);

        Task<MethodResultDTO<ArrivalDTO>> GetSpecificArrival(int idArrival, CancellationToken ct);

        Task<MethodResultDTO> DeleteSpecificReservation(int idReservation, CancellationToken ct);

        Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO, CancellationToken ct);

        Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO, CancellationToken ct);

        Task<MethodResultDTO> ConfirmReservation(int idReservation, CancellationToken ct);
    }
}
