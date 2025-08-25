using hoteru_be.DTOs;
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
    public class DepositController : ControllerBase
    {
        private readonly IDepositQueryService _queries;

        public DepositController(IDepositQueryService queries)
        {
            _queries = queries;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeposit(int id, CancellationToken ct)
        {
            var result = await _queries.GetDeposit(id, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
