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
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetReservations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var result = await _service.GetReservations(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetHistory(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var result = await _service.GetHistory(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }


        [HttpGet("arrivals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetArrivals(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var result = await _service.GetArrivals(page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }


        [HttpGet("history/{idReservation:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificHistory(int idReservation, CancellationToken ct)
        {
            var result = await _service.GetSpecificHistory(idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet("arrival/{idReservation:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificArrival(int idReservation, CancellationToken ct)
        {
            var result = await _service.GetSpecificArrival(idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpDelete("{idReservation:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpecificReservation(int idReservation, CancellationToken ct)
        {
            var result = await _service.DeleteSpecificReservation(idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostReservation([FromBody] PostReservationDTO reservationDTO, CancellationToken ct)
        {
            var result = await _service.PostReservation(reservationDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReservation([FromBody] ArrivalDTO arrivalDTO, CancellationToken ct)
        {
            var result = await _service.UpdateReservation(arrivalDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpPut("confirm/{idReservation:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmReservation(int idReservation, CancellationToken ct)
        {
            var result = await _service.ConfirmReservation(idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
