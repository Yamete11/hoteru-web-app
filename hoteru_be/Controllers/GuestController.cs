using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GuestController : ControllerBase
    {
        private readonly IGuestQueryService _queries;
        private readonly IGuestCommandService _commands;

        public GuestController(IGuestQueryService queries, IGuestCommandService commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<GuestDTO>>> GetGuests(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? searchField = null,
            CancellationToken ct = default)
        {
            var result = await _queries.GetGuests(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("{idPerson:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpecificGuest([FromRoute] int idPerson, CancellationToken ct = default)
        {
            var result = await _queries.GetSpecificGuest(idPerson, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpDelete("{idPerson:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteGuest([FromRoute] int idPerson, CancellationToken ct = default)
        {
            var result = await _commands.DeleteGuest(idPerson, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateGuest([FromBody] GuestDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return StatusCode(StatusCodes.Status400BadRequest,
                    MethodResultDTO.BadRequest("Validation failed", errors));
            }

            var result = await _commands.UpdateGuest(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostGuest([FromBody] GuestDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return StatusCode(StatusCodes.Status400BadRequest,
                    MethodResultDTO.BadRequest("Validation failed", errors));
            }

            var result = await _commands.PostGuest(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
