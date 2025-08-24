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
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _service;

        public RoomTypeController(IRoomTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetRoomTypes(CancellationToken ct)
        {
            var list = await _service.GetRoomTypes(ct);
            return Ok(list);
        }
    }
}
