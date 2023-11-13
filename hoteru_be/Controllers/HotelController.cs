using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _service;

        public HotelController(IHotelService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostOrder([FromBody] HotelDTO hotelDTO)
        {
            return await _service.PostHotel(hotelDTO);
        }

    }
}
