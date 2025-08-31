using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public class AuthCommandService : IAuthCommandService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _hasher;
        private readonly ILogger<AuthCommandService> _logger;

        public AuthCommandService(MyDbContext context, IConfiguration config, IPasswordHasher<User> hasher, ILogger<AuthCommandService> logger)
        {
            _context = context;
            _config = config;
            _hasher = hasher;
            _logger = logger;
        }

        private static string GenerateRefreshRaw()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[64];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        private static string Hash(string raw) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

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

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 30;
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.IdPerson.ToString()),
                new(ClaimTypes.NameIdentifier, user.IdPerson.ToString()),
                new(ClaimTypes.Name, user.LoginName),
                new(ClaimTypes.Role, user.UserType.Title),
                new("role", user.UserType.Title),
                new("hotelId", user.Person.IdHotel.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var keysArr = _config.GetSection("Jwt:Keys").Get<string[]>();
            var currentKey = (keysArr != null && keysArr.Length > 0) ? keysArr[0] : _config["Jwt:Key"]
                             ?? throw new InvalidOperationException("Jwt key is missing");
            var sk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(currentKey)) { KeyId = "k0" };
            var creds = new SigningCredentials(sk, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(minutes),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshDays = int.TryParse(_config["Jwt:RefreshTokenDays"], out var d) ? d : 14;
            var rawRefresh = GenerateRefreshRaw();
            var rt = new RefreshToken
            {
                IdPerson = user.IdPerson,
                TokenHash = Hash(rawRefresh),
                CreatedUtc = now,
                ExpiresUtc = now.AddDays(refreshDays),
                CreatedByIp = null,
                UserAgent = null
            };
            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync(ct);

            return MethodResultDTO<AuthResponseDTO>.Ok(
                new AuthResponseDTO { Token = accessToken, ExpiresAtUtc = token.ValidTo },
                message: rawRefresh);
        }

        public async Task<MethodResultDTO<AuthResponseDTO>> RefreshAsync(string rawRefresh, string? ip, string? userAgent, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(rawRefresh))
            {
                return MethodResultDTO<AuthResponseDTO>.Unauthorized("Missing refresh token");
            }

            var hash = Hash(rawRefresh);
            var now = DateTime.UtcNow;

            var rt = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
            if (rt == null || !rt.IsActive)
            {
                return MethodResultDTO<AuthResponseDTO>.Unauthorized("Invalid refresh token");
            }

            rt.RevokedUtc = now;

            var refreshDays = int.TryParse(_config["Jwt:RefreshTokenDays"], out var d) ? d : 14;
            var newRaw = GenerateRefreshRaw();
            var newRt = new RefreshToken
            {
                IdPerson = rt.IdPerson,
                TokenHash = Hash(newRaw),
                CreatedUtc = now,
                ExpiresUtc = now.AddDays(refreshDays),
                CreatedByIp = ip,
                UserAgent = userAgent
            };
            rt.ReplacedByTokenHash = newRt.TokenHash;
            _context.RefreshTokens.Add(newRt);

            var user = await _context.Users
                .Include(u => u.UserType)
                .Include(u => u.Person)
                .SingleAsync(u => u.IdPerson == rt.IdPerson, ct);

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 30;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.IdPerson.ToString()),
                new(ClaimTypes.NameIdentifier, user.IdPerson.ToString()),
                new(ClaimTypes.Name, user.LoginName),
                new(ClaimTypes.Role, user.UserType.Title),
                new("hotelId", user.Person.IdHotel.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var keysArr = _config.GetSection("Jwt:Keys").Get<string[]>();
            var currentKey = (keysArr != null && keysArr.Length > 0) ? keysArr[0] : _config["Jwt:Key"]
                             ?? throw new InvalidOperationException("Jwt key is missing");
            var sk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(currentKey)) { KeyId = "k0" };
            var creds = new SigningCredentials(sk, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(issuer, audience, claims, now, now.AddMinutes(minutes), creds);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            await _context.SaveChangesAsync(ct);

            return MethodResultDTO<AuthResponseDTO>.Ok(
                new AuthResponseDTO { Token = accessToken, ExpiresAtUtc = jwt.ValidTo },
                message: newRaw);
        }

        public async Task<MethodResultDTO> RevokeRefreshAsync(string rawRefresh, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(rawRefresh))
                return MethodResultDTO.BadRequest("Missing refresh token");

            var rt = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == Hash(rawRefresh), ct);
            if (rt == null || !rt.IsActive) return MethodResultDTO.Ok("Already revoked");

            rt.RevokedUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
            return MethodResultDTO.Ok("Revoked");
        }
    }
}
