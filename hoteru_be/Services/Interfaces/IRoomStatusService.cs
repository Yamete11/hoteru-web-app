using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IRoomStatusService
    {
        public Task<IEnumerable<RoomStatusDTO>> GetRoomStatuses();
    }
}
