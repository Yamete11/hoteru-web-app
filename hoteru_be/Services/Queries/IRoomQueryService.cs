using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IRoomQueryService
    {
        Task<PaginatedResultDTO<RoomDTO>> GetRooms(
            int page,
            int limit,
            string searchQuery = "",
            string searchField = "number",
            CancellationToken ct = default);

        Task<List<RoomDTO>> GetFreeRooms(int idRoom, CancellationToken ct = default);

        Task<MethodResultDTO<SpecificRoomDTO>> GetSpecificRoom(int idRoom, CancellationToken ct = default);
    }
}
