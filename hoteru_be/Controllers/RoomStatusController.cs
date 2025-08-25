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
    public class RoomStatusController : ControllerBase
    {
        private readonly IRoomStatusQueryService _service;

        public RoomStatusController(IRoomStatusQueryService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<StatusDTO>>> GetRoomStatuses(CancellationToken ct)
        {
            var list = await _service.GetRoomStatuses(ct);
            return Ok(list);
        }
    }
}
