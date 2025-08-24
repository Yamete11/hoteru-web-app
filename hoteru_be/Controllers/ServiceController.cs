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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ServiceDTO>>> GetServices(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string? searchField = null,
            [FromQuery] string? searchQuery = null,
            CancellationToken ct = default)
        {
            var result = await _service.GetServices(page, limit, searchField ?? "", searchQuery ?? "", ct);
            return Ok(result);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificService(int id, CancellationToken ct)
        {
            var result = await _service.GetSpecificService(id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostService([FromBody] ServiceDTO dto, CancellationToken ct)
        {
            var result = await _service.PostService(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDTO dto, CancellationToken ct)
        {
            dto.IdService = id;
            var result = await _service.UpdateService(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteService(int id, CancellationToken ct)
        {
            var result = await _service.DeleteService(id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
