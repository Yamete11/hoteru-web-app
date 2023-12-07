using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<IEnumerable<ReservationDTO>> GetReservations();
        public Task<IEnumerable<ReservationDTO>> GetHistory();
        public Task<IEnumerable<ReservationDTO>> GetArrivals();

        public Task<IEnumerable<FullReservationDTO>> GetSpecificHistory(int IdReservation);

        public Task<MethodResultDTO> DeleteSpecificReservation(int IdReservation);
    }
}
