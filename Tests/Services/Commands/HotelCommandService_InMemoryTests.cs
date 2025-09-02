using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Commands
{
    public class HotelCommandService_InMemoryTests
    {
        private static MyDbContext CreateCtx(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new MyDbContext(options);
        }

        private static void SeedUserTypes(MyDbContext ctx)
        {
            ctx.UserTypes.AddRange(
                new UserType { IdUserType = 1, Title = "Superadmin" },
                new UserType { IdUserType = 2, Title = "Admin" },
                new UserType { IdUserType = 3, Title = "Employee" }
            );
            ctx.SaveChanges();
        }

        private static HotelCommandService CreateSut(MyDbContext ctx, out Mock<IEmailCommandService> emailMock)
        {
            emailMock = new Mock<IEmailCommandService>();
            var logger = new Mock<ILogger<HotelCommandService>>();
            return new HotelCommandService(ctx, emailMock.Object, logger.Object);
        }

        [Fact]
        public async Task PostHotel_ShouldCreate_AllEntities_AndSendEmail()
        {
            using var ctx = CreateCtx(nameof(PostHotel_ShouldCreate_AllEntities_AndSendEmail));
            SeedUserTypes(ctx);
            var sut = CreateSut(ctx, out var email);

            var dto = new NewHotelDTO
            {
                Title = "H1",
                Country = "PL",
                City = "W",
                Street = "S1",
                Postcode = "00-001",
                Name = "A",
                Surname = "B",
                Email = "ab@x.com",
                LoginName = "admin_h1",
                Password = "p@ss"
            };

            var r = await sut.PostHotel(dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Hotels.Any(h => h.Title == "H1").Should().BeTrue();
            ctx.Addresses.Any(a => a.City == "W" && a.Postcode == "00-001").Should().BeTrue();
            ctx.Persons.Any(p => p.Email == "ab@x.com").Should().BeTrue();
            var user = ctx.Users.Single(u => u.LoginName == "admin_h1");
            user.IdUserType.Should().Be(ctx.UserTypes.Single(t => t.Title == "Admin").IdUserType);
            user.Password.Should().NotBeNullOrEmpty();
            email.Verify(x => x.SendEmailAsync("ab@x.com",
                                               It.Is<string>(s => s.Contains("Welcome")),
                                               It.Is<string>(b => b.Contains("H1") && b.Contains("admin_h1")),
                                               It.IsAny<CancellationToken>()),
                         Times.Once);
        }

        [Fact]
        public async Task PostHotel_ShouldBadRequest_OnDuplicates()
        {
            using var ctx = CreateCtx(nameof(PostHotel_ShouldBadRequest_OnDuplicates));
            SeedUserTypes(ctx);

            var addr = new Address { IdAddress = 1, City = "C", Country = "PL", Street = "S", Postcode = "00-000" };
            var hotel = new Hotel { IdHotel = 1, Title = "DupHotel", Address = addr };
            ctx.Addresses.Add(addr);
            ctx.Hotels.Add(hotel);
            var person = new Person { IdPerson = 1, Name = "N", Surname = "S", Email = "dup@x.com", Hotel = hotel, IdHotel = 1 };
            ctx.Persons.Add(person);
            ctx.Users.Add(new User { IdPerson = 1, Person = person, IdUserType = 2, LoginName = "dup_login", Password = "x" });
            ctx.SaveChanges();

            var sut = CreateSut(ctx, out _);

            var dto = new NewHotelDTO
            {
                Title = "DupHotel",
                Country = "PL",
                City = "W",
                Street = "S",
                Postcode = "00-001",
                Name = "A",
                Surname = "B",
                Email = "dup@x.com",
                LoginName = "dup_login",
                Password = "p"
            };

            var r = await sut.PostHotel(dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            r.Errors.Should().ContainKey("Email");
            r.Errors.Should().ContainKey("LoginName");
            r.Errors.Should().ContainKey("Title");
        }

        [Fact]
        public async Task DeleteHotel_ShouldOk_WhenExists()
        {
            using var ctx = CreateCtx(nameof(DeleteHotel_ShouldOk_WhenExists));
            SeedUserTypes(ctx);
            ctx.Addresses.Add(new Address { IdAddress = 10, City = "X", Country = "PL", Street = "S", Postcode = "11-111" });
            ctx.Hotels.Add(new Hotel { IdHotel = 10, Title = "ToDelete", IdAddress = 10 });
            ctx.SaveChanges();

            var sut = CreateSut(ctx, out _);

            var r = await sut.DeleteHotel("ToDelete", CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            ctx.Hotels.Any(h => h.Title == "ToDelete").Should().BeFalse();
        }

        [Fact]
        public async Task DeleteHotel_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(DeleteHotel_ShouldNotFound_WhenMissing));
            SeedUserTypes(ctx);
            var sut = CreateSut(ctx, out _);

            var r = await sut.DeleteHotel("Nope", CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteHotel_ShouldBadRequest_WhenEmptyTitle()
        {
            using var ctx = CreateCtx(nameof(DeleteHotel_ShouldBadRequest_WhenEmptyTitle));
            SeedUserTypes(ctx);
            var sut = CreateSut(ctx, out _);

            var r = await sut.DeleteHotel("  ", CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateHotel_ShouldUpdate_Title_And_Address()
        {
            using var ctx = CreateCtx(nameof(UpdateHotel_ShouldUpdate_Title_And_Address));
            SeedUserTypes(ctx);

            var addr = new Address { IdAddress = 1, City = "C1", Country = "PL", Street = "S1", Postcode = "00-001" };
            var hotel = new Hotel { IdHotel = 1, Title = "Old", Address = addr };
            ctx.Addresses.Add(addr);
            ctx.Hotels.Add(hotel);
            ctx.SaveChanges();

            var sut = CreateSut(ctx, out _);

            var dto = new HotelDTO
            {
                Title = "New",
                City = "C2",
                Country = "PL2",
                Street = "S2",
                Postcode = "00-002"
            };

            var r = await sut.UpdateHotel(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var h = ctx.Hotels.Include(x => x.Address).Single(x => x.IdHotel == 1);
            h.Title.Should().Be("New");
            h.Address.City.Should().Be("C2");
            h.Address.Country.Should().Be("PL2");
            h.Address.Street.Should().Be("S2");
            h.Address.Postcode.Should().Be("00-002");
        }

        [Fact]
        public async Task UpdateHotel_ShouldBadRequest_OnDuplicateTitle()
        {
            using var ctx = CreateCtx(nameof(UpdateHotel_ShouldBadRequest_OnDuplicateTitle));
            SeedUserTypes(ctx);

            var a1 = new Address { IdAddress = 1, City = "C1", Country = "PL", Street = "S1", Postcode = "00-001" };
            var a2 = new Address { IdAddress = 2, City = "C2", Country = "PL", Street = "S2", Postcode = "00-002" };
            var h1 = new Hotel { IdHotel = 1, Title = "A", Address = a1 };
            var h2 = new Hotel { IdHotel = 2, Title = "B", Address = a2 };
            ctx.Addresses.AddRange(a1, a2);
            ctx.Hotels.AddRange(h1, h2);
            ctx.SaveChanges();

            var sut = CreateSut(ctx, out _);

            var dto = new HotelDTO { Title = "B", City = "X", Country = "Y", Street = "Z", Postcode = "11-111" };

            var r = await sut.UpdateHotel(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            r.Errors.Should().ContainKey("Title");
        }

        [Fact]
        public async Task UpdateHotel_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(UpdateHotel_ShouldNotFound_WhenMissing));
            SeedUserTypes(ctx);
            var sut = CreateSut(ctx, out _);

            var dto = new HotelDTO { Title = "X" };

            var r = await sut.UpdateHotel(999, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostHotel_ShouldPickFirstUserType_WhenNoAdminExists()
        {
            using var ctx = CreateCtx(nameof(PostHotel_ShouldPickFirstUserType_WhenNoAdminExists));
            ctx.UserTypes.AddRange(
                new UserType { IdUserType = 5, Title = "Alpha" },
                new UserType { IdUserType = 6, Title = "Beta" }
            );
            ctx.SaveChanges();

            var sut = CreateSut(ctx, out _);

            var dto = new NewHotelDTO
            {
                Title = "T",
                Country = "PL",
                City = "C",
                Street = "S",
                Postcode = "00-000",
                Name = "N",
                Surname = "S",
                Email = "n@x.com",
                LoginName = "login",
                Password = "pwd"
            };

            var r = await sut.PostHotel(dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Users.Single(u => u.LoginName == "login").IdUserType.Should().Be(5);
        }
    }
}
