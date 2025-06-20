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
    public class DepositTypeController : ControllerBase
    {
        private readonly IDepositTypeService _service;

        public DepositTypeController(IDepositTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<TypeDTO>> GetDepositTypes()
        {
            return await _service.GetDepositTypes();
        }
    }
}
