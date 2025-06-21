using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace hoteru_be.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<PaginatedResultDTO<RoomDTO>>> GetRooms([FromQuery] int page = 1, [FromQuery] int limit = 15, [FromQuery] string searchQuery = "", [FromQuery] string searchField = "number")
        {
            var result = await _service.GetRooms(page, limit, searchQuery, searchField);
            return Ok(result);
        }



        [HttpGet("freeRooms")]
        public async Task<List<RoomDTO>> GetFreeRooms([FromQuery] int idRoom = 0)
        {
            return await _service.GetFreeRooms(idRoom);
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
        public async Task<MethodResultDTO> UpdateRoom([FromBody] RoomDTO roomDTO)
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
