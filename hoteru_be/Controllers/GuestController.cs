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
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _service;

        public GuestController(IGuestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<GuestDTO>> GetGuests()
        {
            return await _service.GetGuests();
        }
    }
}
