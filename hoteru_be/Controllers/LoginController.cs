using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IAuthService _authService;

    public LoginController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LoginDTO loginDTO)
    {
        var result = await _authService.AuthenticateAsync(loginDTO);

        if (!result.Success)
            return Unauthorized(new { message = result.ErrorMessage });

        return Ok(new
        {
            Token = result.Token,
            Type = "Bearer",
            Expiry = DateTime.UtcNow.AddMinutes(120),
            message = "Login works fine"
        });
    }
}
