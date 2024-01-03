using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hoteru_be.Services.Interfaces;
using hoteru_be.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [HttpGet]
        public Task<List<ListUserDTO>> GetUsers()
        {
            return _service.GetUsers();
        }

        [HttpPost]
        public async Task<MethodResultDTO> PostUser([FromBody] NewUserDTO newUserDTO)
        {
            return await _service.PostUser(newUserDTO);
        }

        [HttpPut]
        public async Task<MethodResultDTO> UpdateUser([FromBody] NewUserDTO newUserDTO)
        {
            return await _service.UpdateUser(newUserDTO);
        }

        [HttpDelete("{IdPerson}")]
        public async Task<MethodResultDTO> DeleteUser(int IdPerson)
        {
            return await _service.DeleteUser(IdPerson);
        }
    }
}
