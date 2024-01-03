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
    public class UserTypeController : ControllerBase
    {
        private readonly IUserTypeService _service;

        public UserTypeController(IUserTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<TypeDTO>> GetUserTypes()
        {
            return await _service.GetUserTypes();
        }
    }
}
