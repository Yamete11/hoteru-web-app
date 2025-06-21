using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace hoteru_be.Controllers
{
    [Authorize]
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
        public async Task<PaginatedResultDTO<ServiceDTO>> GetServices([FromQuery] int page = 1, [FromQuery] int limit = 15, [FromQuery] string searchField = null, [FromQuery] string searchQuery = null)
        {
            return await _service.GetServices(page, limit, searchField, searchQuery);
        }


        [HttpGet("{idService}")]
        public async Task<ServiceDTO> GetSpecificService(int idService)
        {
            return await _service.GetSpecificService(idService);
        }

        [HttpDelete("{IdService}")]
        public async Task<MethodResultDTO> DeleteService(int IdService)
        {
            return await _service.DeleteService(IdService);
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostService([FromBody] ServiceDTO serviceDTO)
        {
            return await _service.PostService(serviceDTO);
        }

        [HttpPut]
        public async Task<MethodResultDTO> UpdateService([FromBody] ServiceDTO serviceDTO)
        {
            return await _service.UpdateService(serviceDTO);
        }
    }
}
