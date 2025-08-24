using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public interface IRoomService
    {
        Task<PaginatedResultDTO<RoomDTO>> GetRooms(int page, int limit, string searchQuery = "", string searchField = "number", CancellationToken ct = default);
        Task<List<RoomDTO>> GetFreeRooms(int idRoom, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteRoom(int IdRoom, CancellationToken ct);
        Task<MethodResultDTO> PostRoom(RoomDTO roomDTO, CancellationToken ct);
        Task<MethodResultDTO<SpecificRoomDTO>> GetSpecificRoom(int IdRoom, CancellationToken ct);
        Task<MethodResultDTO> UpdateRoom(RoomDTO roomDTO, CancellationToken ct);
    }
}
