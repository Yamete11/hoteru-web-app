using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace hoteru_be.UnitTests
{
    public class AuthServiceTests
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthServiceTests()
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            var context = new MyDbContext(options);

            var userType = new UserType { Title = "User" };
            context.UserTypes.Add(userType);
            context.SaveChanges();

            var user = new User
            {
                LoginName = "testuser",
                UserType = userType,
                IdPerson = 1
            };
            user.Password = _passwordHasher.HashPassword(user, "password");

            context.Users.Add(user);
            context.SaveChanges();

            return context;
        }

        private IConfiguration GetConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "MySuperSecretKey12345"},
                {"Jwt:Issuer", "TestIssuer"},
                {"Jwt:Audience", "TestAudience"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return configuration;
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccessAndToken()
        {
            var context = GetInMemoryDbContext();
            var config = GetConfiguration();
            var service = new AuthService(context, config, _passwordHasher);

            var loginDto = new LoginDTO { Login = "testuser", Password = "password" };

            var result = await service.AuthenticateAsync(loginDto);

            Assert.True(result.Success);
            Assert.NotNull(result.Token);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Token);
            var claims = token.Claims.ToList();

            Assert.Contains(claims, c => c.Type == "unique_name" && c.Value == "testuser");
            Assert.Contains(claims, c => c.Type == "nameid" && c.Value == "1");
            Assert.Contains(claims, c => c.Type == "role" && c.Value == "User");
        }
    }
}
