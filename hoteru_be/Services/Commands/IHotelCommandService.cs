using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public interface IHotelCommandService
    {
        Task<MethodResultDTO> PostHotel(HotelDTO hotelDTO, CancellationToken ct);

        Task<MethodResultDTO> DeleteHotel(string hotelTitle, CancellationToken ct);
    }
}
