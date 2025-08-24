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
    public class DepositController : ControllerBase
    {
        private readonly IDepositService _service;

        public DepositController(IDepositService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeposit(int id, CancellationToken ct)
        {
            var result = await _service.GetDeposit(id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
