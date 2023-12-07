using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
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
        public async Task<IEnumerable<ReservationDTO>> GetReservations()
        {
            return await _service.GetReservations();
        }

        [HttpGet("history")]
        public async Task<IEnumerable<ReservationDTO>> GetHistory()
        {
            return await _service.GetHistory();
        }

        [HttpGet("arrivals")]
        public async Task<IEnumerable<ReservationDTO>> GetArrivals()
        {
            return await _service.GetArrivals();
        }

        [HttpGet("history/{IdReservation}")]
        public async Task<IEnumerable<FullReservationDTO>> GetSpecificHistory(int IdReservation)
        {
            return await _service.GetSpecificHistory(IdReservation);
        }

        [HttpDelete("{IdReservation}")]
        public async Task<MethodResultDTO> DeleteSpecificReservation(int IdReservation)
        {
            return await _service.DeleteSpecificReservation(IdReservation);
        }

    }
}
