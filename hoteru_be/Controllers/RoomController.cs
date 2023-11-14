using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<RoomDTO>> GetRooms([FromQuery] int offset = 0, [FromQuery] int limit = 15)
        {
            return await _service.GetRooms(offset, limit);
        }

        [HttpDelete("{IdRoom}")]
        public async Task<MethodResultDTO> DeleteRoom(int IdRoom)
        {
            return await _service.DeleteRoom(IdRoom);
        }
    }
}
