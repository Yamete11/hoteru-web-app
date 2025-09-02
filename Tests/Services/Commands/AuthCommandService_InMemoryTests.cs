using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Commands
{
    public class AuthCommandService_InMemoryTests
    {
        static MyDbContext CreateCtx(string name)
        {
            var opts = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new MyDbContext(opts);
        }

        static IConfiguration CreateConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "test-issuer",
                    ["Jwt:Audience"] = "test-aud",
                    ["Jwt:AccessTokenMinutes"] = "60",
                    ["Jwt:RefreshTokenDays"] = "7",
                    ["Jwt:Key"] = "supersecret_test_key_32+chars_long!!"
                })
                .Build();

        static void SeedUser(MyDbContext ctx, PasswordHasher<User> hasher,
                             int personId = 10, int hotelId = 1,
                             string login = "user", string password = "p@ss",
                             string roleTitle = "Admin")
        {
            var ut = new UserType { IdUserType = 2, Title = roleTitle };
            var hotel = new Hotel { IdHotel = hotelId, Title = "H", Address = new Address { IdAddress = 1, City = "C", Country = "PL", Street = "S", Postcode = "00-000" } };
            var person = new Person { IdPerson = personId, IdHotel = hotelId, Name = "N", Surname = "S", Email = "u@x.com", Hotel = hotel };
            var user = new User { IdPerson = personId, Person = person, IdUserType = ut.IdUserType, UserType = ut, LoginName = login };
            user.Password = hasher.HashPassword(user, password);

            ctx.UserTypes.Add(ut);
            ctx.Hotels.Add(hotel);
            ctx.Persons.Add(person);
            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        static string Hash(string raw) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

        AuthCommandService CreateSut(MyDbContext ctx, IConfiguration cfg, IPasswordHasher<User>? hasher = null)
        {
            var logger = new Mock<ILogger<AuthCommandService>>().Object;
            return new AuthCommandService(ctx, cfg, hasher ?? new PasswordHasher<User>(), logger);
        }

        [Fact]
        public async Task Authenticate_Should_IssueAccessToken_And_CreateRefresh()
        {
            using var ctx = CreateCtx(nameof(Authenticate_Should_IssueAccessToken_And_CreateRefresh));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, personId: 42, hotelId: 5, login: "ivan", password: "secret", roleTitle: "Admin");
            var sut = CreateSut(ctx, cfg, hasher);

            var res = await sut.AuthenticateAsync(new LoginDTO { Login = "ivan", Password = "secret" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            res.Data.Should().NotBeNull();
            res.Data!.Token.Should().NotBeNullOrWhiteSpace();
            res.Message.Should().NotBeNullOrWhiteSpace();

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(res.Data.Token);
            jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value.Should().Be("42");
            jwt.Claims.First(c => c.Type == "hotelId").Value.Should().Be("5");
            jwt.Claims.Any(c => c.Type == "role" && c.Value == "Admin").Should().BeTrue();

            var saved = ctx.RefreshTokens.Single();
            saved.TokenHash.Should().Be(Hash(res.Message));
            saved.IdPerson.Should().Be(42);
            saved.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task Authenticate_Should_Unauthorized_When_UserMissing()
        {
            using var ctx = CreateCtx(nameof(Authenticate_Should_Unauthorized_When_UserMissing));
            var sut = CreateSut(ctx, CreateConfig());

            var res = await sut.AuthenticateAsync(new LoginDTO { Login = "none", Password = "x" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Authenticate_Should_Unauthorized_When_WrongPassword()
        {
            using var ctx = CreateCtx(nameof(Authenticate_Should_Unauthorized_When_WrongPassword));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, login: "ivan", password: "right");
            var sut = CreateSut(ctx, cfg, hasher);

            var res = await sut.AuthenticateAsync(new LoginDTO { Login = "ivan", Password = "wrong" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_Should_RotateToken_And_IssueNewAccessToken()
        {
            using var ctx = CreateCtx(nameof(Refresh_Should_RotateToken_And_IssueNewAccessToken));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, personId: 7, login: "u", password: "p");
            var sut = CreateSut(ctx, cfg, hasher);

            var auth = await sut.AuthenticateAsync(new LoginDTO { Login = "u", Password = "p" }, CancellationToken.None);
            var oldRaw = auth.Message;

            var res = await sut.RefreshAsync(oldRaw, ip: "1.1.1.1", userAgent: "tests", CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            res.Data!.Token.Should().NotBeNullOrWhiteSpace();
            res.Message.Should().NotBeNullOrWhiteSpace();

            var oldHash = Hash(oldRaw);
            var newHash = Hash(res.Message);

            var old = ctx.RefreshTokens.Single(x => x.TokenHash == oldHash);
            old.RevokedUtc.Should().NotBeNull();
            old.ReplacedByTokenHash.Should().Be(newHash);

            var @new = ctx.RefreshTokens.Single(x => x.TokenHash == newHash && x.IdPerson == 7);
            @new.RevokedUtc.Should().BeNull();
            @new.ExpiresUtc.Should().BeAfter(DateTime.UtcNow);
        }


        [Fact]
        public async Task Refresh_Should_Unauthorized_When_MissingOrNotFound()
        {
            using var ctx = CreateCtx(nameof(Refresh_Should_Unauthorized_When_MissingOrNotFound));
            var sut = CreateSut(ctx, CreateConfig());

            var r1 = await sut.RefreshAsync("", null, null, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var r2 = await sut.RefreshAsync("not-existing", null, null, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_Should_Unauthorized_When_ExpiredOrRevoked()
        {
            using var ctx = CreateCtx(nameof(Refresh_Should_Unauthorized_When_ExpiredOrRevoked));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, personId: 5, login: "x", password: "x");
            var sut = CreateSut(ctx, cfg, hasher);

            var rawExpired = "rawExpired";
            ctx.RefreshTokens.Add(new RefreshToken
            {
                IdPerson = 5,
                TokenHash = Hash(rawExpired),
                CreatedUtc = DateTime.UtcNow.AddDays(-10),
                ExpiresUtc = DateTime.UtcNow.AddDays(-1)
            });

            var rawRevoked = "rawRevoked";
            ctx.RefreshTokens.Add(new RefreshToken
            {
                IdPerson = 5,
                TokenHash = Hash(rawRevoked),
                CreatedUtc = DateTime.UtcNow.AddDays(-1),
                ExpiresUtc = DateTime.UtcNow.AddDays(6),
                RevokedUtc = DateTime.UtcNow.AddHours(-1)
            });
            ctx.SaveChanges();

            var r1 = await sut.RefreshAsync(rawExpired, null, null, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var r2 = await sut.RefreshAsync(rawRevoked, null, null, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Revoke_Should_Ok_When_Revoked()
        {
            using var ctx = CreateCtx(nameof(Revoke_Should_Ok_When_Revoked));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, personId: 3, login: "a", password: "b");
            var sut = CreateSut(ctx, cfg, hasher);

            var auth = await sut.AuthenticateAsync(new LoginDTO { Login = "a", Password = "b" }, CancellationToken.None);
            var raw = auth.Message;

            var r = await sut.RevokeRefreshAsync(raw, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            r.Message.Should().Be("Revoked");
            ctx.RefreshTokens.Single(x => x.TokenHash == Hash(raw)).RevokedUtc.Should().NotBeNull();
        }

        [Fact]
        public async Task Revoke_Should_BeIdempotent()
        {
            using var ctx = CreateCtx(nameof(Revoke_Should_BeIdempotent));
            var cfg = CreateConfig();
            var hasher = new PasswordHasher<User>();
            SeedUser(ctx, hasher, personId: 8, login: "z", password: "z");
            var sut = CreateSut(ctx, cfg, hasher);

            var auth = await sut.AuthenticateAsync(new LoginDTO { Login = "z", Password = "z" }, CancellationToken.None);
            var raw = auth.Message;

            var rt = ctx.RefreshTokens.Single(x => x.TokenHash == Hash(raw));
            rt.RevokedUtc = DateTime.UtcNow;
            ctx.SaveChanges();

            var r = await sut.RevokeRefreshAsync(raw, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            r.Message.Should().Be("Already revoked");
        }
    }
}
