using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IHotelQueryService
    {
        Task<MethodResultDTO<HotelDTO>> GetHotel(int hotelId, CancellationToken ct = default);
    }
}
