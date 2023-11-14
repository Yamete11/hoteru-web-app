using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class GuestService : IGuestService
    {
        public Task<MethodResultDTO> DeleteGuest(int IdGuest)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<GuestDTO>> GetGuests()
        {
            throw new System.NotImplementedException();
        }
    }
}
