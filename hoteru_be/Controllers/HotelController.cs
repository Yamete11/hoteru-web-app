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
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelCommandService _commands;
        private readonly IHotelQueryService _queries;

        public HotelController(IHotelCommandService commands, IHotelQueryService queries)
        {
            _commands = commands;
            _queries = queries;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostHotel([FromBody] NewHotelDTO hotelDTO, CancellationToken ct)
        {
            var result = await _commands.PostHotel(hotelDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        //designed for ui tests, to delete a hotel after testing its creating
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel([FromBody] DeleteHotelRequestDTO request, CancellationToken ct)
        {
            var result = await _commands.DeleteHotel(request.HotelTitle, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [Authorize(Policy = "HasHotelId")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotel(CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetHotel(hotelId, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [Authorize(Policy = "HasHotelId", Roles = "Superadmin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateHotel([FromBody] HotelDTO hotelDTO, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.UpdateHotel(hotelId, hotelDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
