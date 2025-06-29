using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using hoteru_be.Entities;

namespace hoteru_be.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(MyDbContext context, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Token, string ErrorMessage)> AuthenticateAsync(LoginDTO loginDTO)
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.LoginName == loginDTO.Login);

            if (user == null)
                return (false, null, "Login failed. Please check the credentials.");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDTO.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
                return (false, null, "Login failed. Please check the credentials.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var hotelId = user.Person.IdHotel;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.LoginName),
                new Claim(ClaimTypes.NameIdentifier, user.IdPerson.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.Title),
                new Claim("hotelId", hotelId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (true, tokenString, null);
        }
    }
}
