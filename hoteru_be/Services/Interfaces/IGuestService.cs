using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IGuestService
    {
        public Task<IEnumerable<GuestDTO>> GetGuests();

        public Task<SpecificGuestDTO> GetSpecificGuest(int IdPerson);
        public Task<MethodResultDTO> DeleteGuest(int IdPerson);

        public Task<MethodResultDTO> UpdateGuest(SpecificGuestDTO guestDTO);

        public Task<MethodResultDTO> PostGuest(GuestDTO guestDTO);
    }
}
