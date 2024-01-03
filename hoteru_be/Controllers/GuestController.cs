using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _service;

        public GuestController(IGuestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<PaginatedResultDTO<GuestDTO>> GetGuests([FromQuery] int page = 1, [FromQuery] int limit = 15)
        {
            return await _service.GetGuests(page, limit);
        }

        [HttpGet("{IdPerson}")]
        public async Task<SpecificGuestDTO> GetSpecificGuest(int IdPerson)
        {
            return await _service.GetSpecificGuest(IdPerson);
        }

        [HttpDelete("{IdPerson}")]
        public async Task<MethodResultDTO> DeleteGuest(int IdPerson)
        {
            return await _service.DeleteGuest(IdPerson);
        }

        [HttpPut]
        public async Task<MethodResultDTO> UpdateGuest([FromBody] GuestDTO guestDTO)
        {
            return await _service.UpdateGuest(guestDTO);
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostGuest([FromBody] GuestDTO guestDTO)
        {

            return await _service.PostGuest(guestDTO);
        }
    }
}
