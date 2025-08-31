using hoteru_be.DTOs;
using hoteru_be.Services.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthCommandService _commands;
        private readonly int _refreshDays;
        private readonly int _accessMinutes;

        public LoginController(IAuthCommandService commands, IConfiguration config)
        {
            _commands = commands;
            _refreshDays = int.TryParse(config["Jwt:RefreshTokenDays"], out var d) ? d : 14;
            _accessMinutes = int.TryParse(config["Jwt:AccessTokenMinutes"], out var m) ? m : 30;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post([FromBody] LoginDTO dto, CancellationToken ct)
        {
            var result = await _commands.AuthenticateAsync(dto, ct);
            if ((int)result.HttpStatusCode != StatusCodes.Status200OK)
                return StatusCode((int)result.HttpStatusCode, result);

            if (!string.IsNullOrEmpty(result.Data?.Token))
            {
                Response.Cookies.Append("access_token", result.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_accessMinutes)
                });
            }

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

            return Ok(new { expiresAtUtc = result.Data?.ExpiresAtUtc });
        }

        [AllowAnonymous]
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

            if (!string.IsNullOrEmpty(result.Data?.Token))
            {
                Response.Cookies.Append("access_token", result.Data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_accessMinutes)
                });
            }

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

            return Ok(new { expiresAtUtc = result.Data?.ExpiresAtUtc });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            if (Request.Cookies.TryGetValue("rt", out var raw))
                await _commands.RevokeRefreshAsync(raw, ct);

            var del = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UnixEpoch
            };
            Response.Cookies.Delete("access_token", del);
            Response.Cookies.Delete("rt", del);

            return Ok(MethodResultDTO.Ok("Logged out"));
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub");

            return Ok(new
            {
                id,
                login = User.Identity?.Name,
                role = User.FindFirstValue(ClaimTypes.Role),
                hotelId = User.FindFirst("hotelId")?.Value
            });
        }

    }
}
