using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using hoteru_be.Services.Common;
using hoteru_be.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Authorize(Policy = "HasHotelId")]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserQueryService _queries;
        private readonly IUserCommandService _commands;

        public UserController(IUserQueryService queries, IUserCommandService commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet("{login}")]
        [ProducesResponseType(typeof(MethodResultDTO<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<UserDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<UserDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser([FromRoute] string login, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetUser(hotelId, login, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet("fullUser/{idUser:int}")]
        [ProducesResponseType(typeof(MethodResultDTO<FullUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<FullUserDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<FullUserDTO>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFullUser([FromRoute] int idUser, CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetFullUser(hotelId, idUser, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Superadmin")]
        [ProducesResponseType(typeof(MethodResultDTO<List<ListUserDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO<List<ListUserDTO>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO<List<ListUserDTO>>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var hotelId = User.GetHotelId();
            var result = await _queries.GetUsers(hotelId, ct);
            return StatusCode((int)result.HttpStatusCode, result);
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
            var hotelId = User.GetHotelId();
            var result = await _commands.PostUser(hotelId, dto, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }

        [HttpPut]
        [Authorize]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(MethodResultDTO), StatusCodes.Status422UnprocessableEntity)]
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
            var hotelId = User.GetHotelId();
            var result = await _commands.UpdateUser(hotelId, dto, ct);
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
            var hotelId = User.GetHotelId();
            var result = await _commands.DeleteUser(hotelId, idPerson, ct);
            return StatusCode((int)result.HttpStatusCode, result);
        }
    }
}
