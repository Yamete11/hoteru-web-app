﻿using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestStatusController : ControllerBase
    {
        private readonly IGuestStatusService _service;

        public GuestStatusController(IGuestStatusService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IEnumerable<GuestStatusDTO>> GetRoomStatuses()
        {
            return await _service.GetGuestStatuses();
        }
    }
}