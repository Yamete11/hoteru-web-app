using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GuestStatusController : ControllerBase
    {
        private readonly IGuestStatusQueryService _service;

        public GuestStatusController(IGuestStatusQueryService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<StatusDTO>>> GetGuestStatuses(CancellationToken ct)
        {
            var list = await _service.GetGuestStatuses(ct);
            return Ok(list);
        }
    }
}
