using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;
using hoteru_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hoteru_be.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] string login, CancellationToken ct)
        {
            var result = await _service.GetUser(login, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet("fullUser/{idUser:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFullUser([FromRoute] int idUser, CancellationToken ct)
        {
            var result = await _service.GetFullUser(idUser, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Superadmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<ListUserDTO>>> GetUsers(CancellationToken ct)
        {
            var users = await _service.GetUsers(ct);
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Superadmin")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PostUser([FromBody] NewUserDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return StatusCode(StatusCodes.Status400BadRequest,
                    MethodResultDTO.BadRequest("Validation failed", errors));
            }

            var result = await _service.PostUser(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut]
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return StatusCode(StatusCodes.Status400BadRequest,
                    MethodResultDTO.BadRequest("Validation failed", errors));
            }

            var result = await _service.UpdateUser(dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpDelete("{idPerson:int}")]
        [Authorize(Roles = "Admin,Superadmin")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] int idPerson, CancellationToken ct)
        {
            var result = await _service.DeleteUser(idPerson, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
