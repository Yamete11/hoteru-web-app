using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomQueryService _queries;
        private readonly IRoomCommandService _commands;

        public RoomController(IRoomQueryService queries, IRoomCommandService commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResultDTO<RoomDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<RoomDTO>>> GetRooms(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "number",
            CancellationToken ct = default)
        {
            var result = await _queries.GetRooms(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("freeRooms")]
        [ProducesResponseType(typeof(List<RoomDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<RoomDTO>>> GetFreeRooms([FromQuery] int idRoom = 0, CancellationToken ct = default)
        {
            var list = await _queries.GetFreeRooms(idRoom, ct);
            return Ok(list);
        }

        [HttpGet("{idRoom:int}")]
        [ProducesResponseType(typeof(MethodResultDTO<SpecificRoomDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<SpecificRoomDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<SpecificRoomDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificRoom([FromRoute] int idRoom, CancellationToken ct)
        {
            var result = await _queries.GetSpecificRoom(idRoom, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpDelete("{idRoom:int}")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoom([FromRoute] int idRoom, CancellationToken ct)
        {
            var result = await _commands.DeleteRoom(idRoom, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateRoom([FromBody] RoomDTO roomDTO, CancellationToken ct)
        {
            var result = await _commands.UpdateRoom(roomDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PostRoom([FromBody] RoomDTO roomDTO, CancellationToken ct)
        {
            var result = await _commands.PostRoom(roomDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
