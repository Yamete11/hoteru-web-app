using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<PaginatedResultDTO<ReservationDTO>> GetReservations([FromQuery] int page = 1, [FromQuery] int limit = 15, [FromQuery] string searchQuery = "", [FromQuery] string searchField = "")
        {
            return await _service.GetReservations(page, limit);
        }

        [HttpGet("history")]
        public async Task<PaginatedResultDTO<ReservationDTO>> GetHistory([FromQuery] int page = 1, [FromQuery] int limit = 15, [FromQuery] string searchQuery = "", [FromQuery] string searchField = "")
        {
            return await _service.GetHistory(page, limit);
        }

        [HttpGet("arrivals")]
        public async Task<PaginatedResultDTO<ReservationDTO>> GetArrivals( [FromQuery] int page = 1, [FromQuery] int limit = 15, [FromQuery] string searchQuery = "", [FromQuery] string searchField = "")
        {
            return await _service.GetArrivals(page, limit, searchQuery, searchField);
        }


        [HttpGet("history/{IdReservation}")]
        public async Task<FullReservationDTO> GetSpecificHistory(int IdReservation)
        {
            return await _service.GetSpecificHistory(IdReservation);
        }

        [HttpGet("arrival/{IdReservation}")]
        public async Task<ArrivalDTO> GetSpecificArrival(int IdReservation)
        {
            return await _service.GetSpecificArrival(IdReservation);
        }

        [HttpDelete("{IdReservation}")]
        public async Task<MethodResultDTO> DeleteSpecificReservation(int IdReservation)
        {
            return await _service.DeleteSpecificReservation(IdReservation);
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostReservation([FromBody] PostReservationDTO reservationDTO)
        {

            return await _service.PostReservation(reservationDTO);
        }

        [HttpPut]
        public async Task<MethodResultDTO> UpdateReservation([FromBody] ArrivalDTO arrivalDTO)
        {
            return await _service.UpdateReservation(arrivalDTO);
        }

        [HttpPut("/confirm/{IdReservation}")]
        public async Task<MethodResultDTO> ConfirmReservation(int IdReservation)
        {
            return await _service.ConfirmReservation(IdReservation);
        }



    }
}
