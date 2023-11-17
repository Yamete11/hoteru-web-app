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
    public class RoomStatusController : ControllerBase
    {
        private readonly IRoomStatusService _service;

        public RoomStatusController(IRoomStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<RoomStatusDTO>> GetRoomStatuses()
        {
            return await _service.GetRoomStatuses();
        }
    }
}
