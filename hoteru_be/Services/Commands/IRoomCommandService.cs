using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IRoomCommandService
    {
        Task<MethodResultDTO> PostRoom(RoomDTO roomDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateRoom(RoomDTO roomDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteRoom(int idRoom, CancellationToken ct = default);
    }
}
