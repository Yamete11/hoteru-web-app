using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IRoomService
    {
        public Task<PaginatedResultDTO<RoomDTO>> GetRooms(int page, int limit, string searchQuery = "");

        public Task<List<RoomDTO>> GetFreeRooms(int idRoom);
        public Task<MethodResultDTO> DeleteRoom(int IdRoom);

        public Task<MethodResultDTO> PostRoom(RoomDTO roomDTO);

        public Task<SpecificRoomDTO> GetSpecificRoom(int IdRoom);

        public Task<MethodResultDTO> UpdateRoom(RoomDTO roomDTO);
    }
}
