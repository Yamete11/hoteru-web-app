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
        public async Task<IEnumerable<RoomDTO>> GetRooms([FromQuery] int page = 1, [FromQuery] int limit = 15)
        {
            return await _service.GetRooms(page, limit);
        }

        [HttpGet("{IdRoom}")]
        public async Task<SpecificRoomDTO> GetSpecificRoom(int IdRoom)
        {
            return await _service.GetSpecificRoom(IdRoom);
        }

        [HttpDelete("{IdRoom}")]
        public async Task<MethodResultDTO> DeleteRoom(int IdRoom)
        {
            return await _service.DeleteRoom(IdRoom);
        }

        [HttpPut]
        public async Task<MethodResultDTO> UpdateRoom([FromBody] SpecificRoomDTO roomDTO)
        {
            return await _service.UpdateRoom(roomDTO);
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostRoom([FromBody] RoomDTO roomDTO)
        {
            return await _service.PostRoom(roomDTO);
        }
    }
}
