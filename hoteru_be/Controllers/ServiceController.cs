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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServiceController(IServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<ServiceDTO>> GetServices()
        {
            return await _service.GetServices();
        }

        [HttpDelete("{IdService}")]
        public async Task<MethodResultDTO> DeleteService(int IdService)
        {
            return await _service.DeleteService(IdService);
        }
    }
}