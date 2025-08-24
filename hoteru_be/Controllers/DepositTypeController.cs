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
    public class DepositTypeController : ControllerBase
    {
        private readonly IDepositTypeService _service;

        public DepositTypeController(IDepositTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetDepositTypes(CancellationToken ct)
        {
            var list = await _service.GetDepositTypes(ct);
            return Ok(list);
        }
    }
}
