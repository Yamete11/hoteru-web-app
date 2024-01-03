using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hoteru_be.Services.Interfaces;
using hoteru_be.DTOs;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{login}")]
        public Task<UserDTO> GetUser(string login)
        {
            return _service.GetUser(login);
        }

        [HttpGet("fullUser/{idUser}")]
        public Task<FullUserDTO> GetFullUser(int idUser)
        {
            return _service.GetFullUser(idUser);
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostГыук([FromBody] NewUserDTO newUserDTO)
        {
            return await _service.PostUser(newUserDTO);
        }
    }
}
