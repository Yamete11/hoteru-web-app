using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using hoteru_be.Services.Common;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Authorize(Policy = "HasHotelId")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceQueryService _queries;
        private readonly IServiceCommandService _commands;

        public ServiceController(IServiceQueryService queries, IServiceCommandService commands)
        {
            _queries = queries;
            _commands = commands;
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResultDTO<ServiceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ServiceDTO>>> GetServices(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string? searchField = null,
            [FromQuery] string? searchQuery = null,
            CancellationToken ct = default)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetServices(hotelId, page, limit, searchField ?? "", searchQuery ?? "", ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MethodResultDTO<ServiceDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<ServiceDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<ServiceDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificService([FromRoute] int id, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetSpecificService(hotelId, id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PostService([FromBody] ServiceDTO dto, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.PostService(hotelId, dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateService([FromRoute] int id, [FromBody] ServiceDTO dto, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            dto.IdService = id;
            var result = await _commands.UpdateService(hotelId, dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteService([FromRoute] int id, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.DeleteService(hotelId, id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
