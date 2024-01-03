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
    public class DepositController : ControllerBase
    {
        private readonly IDepositService _service;

        public DepositController(IDepositService service)
        {
            _service = service;
        }

    

        [HttpGet("{IdDeposit}")]
        public async Task<DepositDTO> GetDeposit(int IdDeposit)
        {
            return await _service.GetDeposit(IdDeposit);
        }
    }
}
