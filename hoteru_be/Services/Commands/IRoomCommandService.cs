using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IRoomCommandService
    {
        Task<MethodResultDTO> PostRoom(int hotelId, RoomDTO roomDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateRoom(int hotelId, RoomDTO roomDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteRoom(int hotelId, int idRoom, CancellationToken ct = default);
    }
}
