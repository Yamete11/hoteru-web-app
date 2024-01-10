using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit);
        public Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit);
        public Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit);

        public Task<IEnumerable<FullReservationDTO>> GetSpecificHistory(int IdReservation);

        public Task<ArrivalDTO> GetSpecificArrival(int IdArrival);

        public Task<MethodResultDTO> DeleteSpecificReservation(int IdReservation);

        public Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO);

        public Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO);
    }
}
