using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IReservationCommandService
    {
        Task<MethodResultDTO> PostReservation(int hotelId, PostReservationDTO reservationDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateReservation(int hotelId, ArrivalDTO arrivalDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteSpecificReservation(int hotelId, int idReservation, CancellationToken ct = default);
        Task<MethodResultDTO> ConfirmReservation(int hotelId, int idReservation, CancellationToken ct = default);
    }
}
