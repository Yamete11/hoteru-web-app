using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IReservationCommandService
    {
        Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteSpecificReservation(int idReservation, CancellationToken ct = default);
        Task<MethodResultDTO> ConfirmReservation(int idReservation, CancellationToken ct = default);
    }
}
