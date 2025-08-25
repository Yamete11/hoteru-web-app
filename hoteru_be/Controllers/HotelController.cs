using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelCommandService _commands;

        public HotelController(IHotelCommandService commands)
        {
            _commands = commands;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostHotel([FromBody] HotelDTO hotelDTO, CancellationToken ct)
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
    }
}
