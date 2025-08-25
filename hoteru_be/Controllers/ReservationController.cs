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
    [Authorize(Policy = "HasHotelId")]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationQueryService _queries;
        private readonly IReservationCommandService _commands;

        public ReservationController(IReservationQueryService queries, IReservationCommandService commands)
        {
            _queries = queries;
            _commands = commands;
        }


        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResultDTO<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetReservations(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetReservations(hotelId, page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("history")]
        [ProducesResponseType(typeof(PaginatedResultDTO<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetHistory(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetHistory(hotelId, page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("arrivals")]
        [ProducesResponseType(typeof(PaginatedResultDTO<ReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResultDTO<ReservationDTO>>> GetArrivals(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 15,
            [FromQuery] string searchQuery = "",
            [FromQuery] string searchField = "",
            CancellationToken ct = default)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetArrivals(hotelId, page, limit, searchQuery, searchField, ct);
            return Ok(result);
        }

        [HttpGet("history/{idReservation:int}")]
        [ProducesResponseType(typeof(MethodResultDTO<FullReservationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<FullReservationDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<FullReservationDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificHistory([FromRoute] int idReservation, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetSpecificHistory(hotelId, idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet("arrival/{idReservation:int}")]
        [ProducesResponseType(typeof(MethodResultDTO<ArrivalDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<ArrivalDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<ArrivalDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpecificArrival([FromRoute] int idReservation, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetSpecificArrival(hotelId, idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }


        [HttpDelete("{idReservation:int}")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpecificReservation([FromRoute] int idReservation, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.DeleteSpecificReservation(hotelId, idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PostReservation([FromBody] PostReservationDTO reservationDTO, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.PostReservation(hotelId, reservationDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReservation([FromBody] ArrivalDTO arrivalDTO, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.UpdateReservation(hotelId, arrivalDTO, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut("confirm/{idReservation:int}")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmReservation([FromRoute] int idReservation, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _commands.ConfirmReservation(hotelId, idReservation, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
