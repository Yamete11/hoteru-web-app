using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthCommandService _commands;
        private readonly int _refreshDays;

        public LoginController(IAuthCommandService commands, IConfiguration config)
        {
            _commands = commands;
            _refreshDays = int.TryParse(config["Jwt:RefreshTokenDays"], out var d) ? d : 14;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] LoginDTO dto, CancellationToken ct)
        {
            var result = await _commands.AuthenticateAsync(dto, ct);
            if ((int)result.HttpStatusCode != StatusCodes.Status200OK)
                return StatusCode((int)result.HttpStatusCode, result);

            if (!string.IsNullOrEmpty(result.Message)) 
            {
                Response.Cookies.Append("rt", result.Message, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddDays(_refreshDays)
                });
            }

            return Ok(result.Data);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(CancellationToken ct)
        {
            if (!Request.Cookies.TryGetValue("rt", out var raw))
                return Unauthorized(MethodResultDTO.Unauthorized("Missing refresh token"));

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua = Request.Headers["User-Agent"].ToString();

            var result = await _commands.RefreshAsync(raw, ip, ua, ct);
            if ((int)result.HttpStatusCode != StatusCodes.Status200OK)
                return StatusCode((int)result.HttpStatusCode, result);

            Response.Cookies.Append("rt", result.Message, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(14)
            });

            return Ok(result.Data);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            if (Request.Cookies.TryGetValue("rt", out var raw))
            {
                await _commands.RevokeRefreshAsync(raw, ct);
                Response.Cookies.Delete("rt", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }
            return Ok(MethodResultDTO.Ok("Logged out"));
        }
    }
}
