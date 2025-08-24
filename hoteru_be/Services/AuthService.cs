using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace hoteru_be.Services
{
    public class AuthService : IAuthService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _hasher;
        private readonly ILogger<AuthService> _logger;

        public AuthService(MyDbContext context, IConfiguration config, IPasswordHasher<User> hasher, ILogger<AuthService> logger)
        {
            _context = context;
            _config = config;
            _hasher = hasher;
            _logger = logger;
        }

        public async Task<MethodResultDTO<AuthResponseDTO>> AuthenticateAsync(LoginDTO dto, CancellationToken ct = default)
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .Include(u => u.Person)
                .SingleOrDefaultAsync(u => u.LoginName == dto.Login, ct);

            if (user is null)
            {
                _logger.LogWarning("Auth failed: user {Login} not found", dto.Login);
                return MethodResultDTO<AuthResponseDTO>.Unauthorized("Invalid credentials");
            }

            var verify = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (verify != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Auth failed: wrong password for {Login}", dto.Login);
                return MethodResultDTO<AuthResponseDTO>.Unauthorized("Invalid credentials");
            }

            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var minutesStr = _config["Jwt:AccessTokenMinutes"];
            var minutes = int.TryParse(minutesStr, out var m) ? m : 30;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.IdPerson.ToString()),
                new(ClaimTypes.Name, user.LoginName),
                new(ClaimTypes.Role, user.UserType.Title),
                new("role", user.UserType.Title),
                new("hotelId", user.Person.IdHotel.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return MethodResultDTO<AuthResponseDTO>.Ok(new AuthResponseDTO
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAt
            }, "Authenticated");
        }
    }
}
