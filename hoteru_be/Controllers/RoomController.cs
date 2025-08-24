using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<RoomDTO>>> GetRooms(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "number",
            CancellationToken ct = default)
        {
            var result = await _service.GetRooms(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("freeRooms")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RoomDTO>>> GetFreeRooms([FromQuery] int idRoom = 0, CancellationToken ct = default)
        {
            var list = await _service.GetFreeRooms(idRoom, ct);
            return Ok(list);
        }

        [HttpGet("{idRoom:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificRoom(int idRoom, CancellationToken ct)
        {
            var result = await _service.GetSpecificRoom(idRoom, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpDelete("{idRoom:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom(int idRoom, CancellationToken ct)
        {
            var result = await _service.DeleteRoom(idRoom, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDTO roomDTO, CancellationToken ct)
        {
            var result = await _service.UpdateRoom(roomDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostRoom([FromBody] RoomDTO roomDTO, CancellationToken ct)
        {
            var result = await _service.PostRoom(roomDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
