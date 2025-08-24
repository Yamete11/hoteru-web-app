using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _service;

        public HotelController(IHotelService service)
        {
            _service = service;
        }


        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostHotel([FromBody] HotelDTO hotelDTO, CancellationToken ct)
        {
            var result = await _service.PostHotel(hotelDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel([FromBody] DeleteHotelRequestDTO request, CancellationToken ct)
        {
            var result = await _service.DeleteHotel(request.HotelTitle, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
