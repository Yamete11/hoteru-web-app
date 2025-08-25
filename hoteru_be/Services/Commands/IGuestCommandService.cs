using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IGuestCommandService
    {
        Task<MethodResultDTO> PostGuest(int hotelId, GuestDTO guestDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateGuest(int hotelId, GuestDTO guestDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteGuest(int hotelId, int idPerson, CancellationToken ct = default);
    }
}
