using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IGuestService
    {
        public Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string searchQuery = null, string searchField = null);

        public Task<SpecificGuestDTO> GetSpecificGuest(int IdPerson);
        public Task<MethodResultDTO> DeleteGuest(int IdPerson);

        public Task<MethodResultDTO> UpdateGuest(GuestDTO guestDTO);

        public Task<MethodResultDTO> PostGuest(GuestDTO guestDTO);
    }
}
