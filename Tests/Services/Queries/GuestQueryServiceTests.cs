using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.Entities;
using hoteru_be.Services.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Queries
{
    public class GuestQueryService_InMemoryTests
    {
        private static MyDbContext CreateCtx(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var ctx = new MyDbContext(options);
            return ctx;
        }

        private static void Seed(MyDbContext ctx)
        {
            ctx.GuestStatuses.AddRange(
                new GuestStatus { IdGuestStatus = 1, Title = "Active" },
                new GuestStatus { IdGuestStatus = 2, Title = "Blocked" }
            );

            var p1 = new Person { IdPerson = 1, IdHotel = 1, Name = "Ivan", Surname = "Ivanov", Email = "ivan@x.com" };
            var p2 = new Person { IdPerson = 2, IdHotel = 1, Name = "Petr", Surname = "Petrov", Email = "petr@x.com" };
            var p3 = new Person { IdPerson = 3, IdHotel = 2, Name = "Olga", Surname = "Sidorova", Email = "olga@x.com" };

            ctx.Persons.AddRange(p1, p2, p3);

            ctx.Guests.AddRange(
                new Guest { IdPerson = 1, Person = p1, Passport = "P1", TelNumber = "111", IdGuestStatus = 1, GuestStatus = ctx.GuestStatuses.Find(1) },
                new Guest { IdPerson = 2, Person = p2, Passport = "P2", TelNumber = "222", IdGuestStatus = 2, GuestStatus = ctx.GuestStatuses.Find(2) },
                new Guest { IdPerson = 3, Person = p3, Passport = "P3", TelNumber = "333", IdGuestStatus = 1, GuestStatus = ctx.GuestStatuses.Find(1) }
            );

            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetGuests_ShouldReturnPagedList_WithoutFilter()
        {
            using var ctx = CreateCtx(nameof(GetGuests_ShouldReturnPagedList_WithoutFilter));
            Seed(ctx);

            var logger = new Mock<ILogger<GuestQueryService>>();
            var sut = new GuestQueryService(ctx, logger.Object);

            var result = await sut.GetGuests(hotelId: 1, page: 1, limit: 1, ct: CancellationToken.None);

            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.Limit.Should().Be(1);
            result.List.Should().HaveCount(1);
            var g = result.List.First();
            g.IdPerson.Should().Be(1);
            g.Name.Should().Be("Ivan");
            g.Surname.Should().Be("Ivanov");
            g.Email.Should().Be("ivan@x.com");
            g.Passport.Should().Be("P1");
            g.TelNumber.Should().Be("111");
            g.IdGuestStatus.Should().Be("Active");
        }

        [Fact]
        public async Task GetGuests_ShouldNormalizePageAndLimit_WhenZeroOrNegative()
        {
            using var ctx = CreateCtx(nameof(GetGuests_ShouldNormalizePageAndLimit_WhenZeroOrNegative));
            Seed(ctx);

            var logger = new Mock<ILogger<GuestQueryService>>();
            var sut = new GuestQueryService(ctx, logger.Object);

            var result = await sut.GetGuests(hotelId: 1, page: 0, limit: 0, ct: CancellationToken.None);

            result.Page.Should().Be(1);
            result.Limit.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.List.Should().HaveCount(2);
            result.List.Select(x => x.IdPerson).Should().Equal(1, 2);
        }

        [Fact]
        public async Task GetSpecificGuest_ShouldReturnOk_WhenFound()
        {
            using var ctx = CreateCtx(nameof(GetSpecificGuest_ShouldReturnOk_WhenFound));
            Seed(ctx);

            var logger = new Mock<ILogger<GuestQueryService>>();
            var sut = new GuestQueryService(ctx, logger.Object);

            var result = await sut.GetSpecificGuest(hotelId: 1, idPerson: 2, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.IdPerson.Should().Be(2);
            result.Data.Name.Should().Be("Petr");
            result.Data.Surname.Should().Be("Petrov");
            result.Data.Email.Should().Be("petr@x.com");
            result.Data.Passport.Should().Be("P2");
            result.Data.TelNumber.Should().Be("222");
            result.Data.IdGuestStatus.Should().Be(2);
        }

        [Fact]
        public async Task GetSpecificGuest_ShouldReturnNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(GetSpecificGuest_ShouldReturnNotFound_WhenMissing));
            Seed(ctx);

            var logger = new Mock<ILogger<GuestQueryService>>();
            var sut = new GuestQueryService(ctx, logger.Object);

            var result = await sut.GetSpecificGuest(hotelId: 1, idPerson: 999, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be("Guest not found");
            result.Data.Should().BeNull();
        }
    }
}
