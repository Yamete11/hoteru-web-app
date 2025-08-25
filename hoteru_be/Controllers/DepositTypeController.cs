using hoteru_be.DTOs;
using hoteru_be.Services.Common;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Authorize(Policy = "HasHotelId")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepositTypeController : ControllerBase
    {
        private readonly IDepositTypeQueryService _queries;

        public DepositTypeController(IDepositTypeQueryService queries)
        {
            _queries = queries;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetDepositTypes(CancellationToken ct)
        {
            var list = await _queries.GetDepositTypes(ct);
            return Ok(list);
        }
    }
}
