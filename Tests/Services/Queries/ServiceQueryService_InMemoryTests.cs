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
    public class ServiceQueryService_InMemoryTests
    {
        private static MyDbContext CreateCtx(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new MyDbContext(options);
        }

        private static void Seed(MyDbContext ctx)
        {
            var ut = new UserType { IdUserType = 1, Title = "Manager" };
            ctx.UserTypes.Add(ut);

            var p1 = new Person { IdPerson = 1, IdHotel = 1, Name = "U1", Surname = "S1", Email = "u1@x.com" };
            var p2 = new Person { IdPerson = 2, IdHotel = 2, Name = "U2", Surname = "S2", Email = "u2@x.com" };
            ctx.Persons.AddRange(p1, p2);

            var u1 = new User { IdPerson = 1, Person = p1, IdUserType = 1, UserType = ut, LoginName = "u1", Password = "pwd" };
            var u2 = new User { IdPerson = 2, Person = p2, IdUserType = 1, UserType = ut, LoginName = "u2", Password = "pwd" };
            ctx.Users.AddRange(u1, u2);

            ctx.Services.AddRange(
                new Service { IdService = 1, Title = "Spa access", Description = "Full day", Sum = 30m, User = u1 },
                new Service { IdService = 2, Title = "Breakfast", Description = "Buffet", Sum = 15m, User = u1 },
                new Service { IdService = 3, Title = "Airport transfer", Description = null, Sum = 50m, User = u2 }
            );

            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetServices_ShouldReturnPaged_ForHotel()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldReturnPaged_ForHotel));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 1, limit: 1, searchField: "", searchQuery: "", ct: CancellationToken.None);

            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.Limit.Should().Be(1);
            result.List.Should().HaveCount(1);
            result.List.First().IdService.Should().Be(1);
            result.List.First().Title.Should().Be("Spa access");
            result.List.First().Sum.Should().Be(30m);
            result.List.First().Description.Should().Be("Full day");
        }

        [Fact]
        public async Task GetServices_ShouldNormalizePageAndLimit_WhenZeroOrNegative()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldNormalizePageAndLimit_WhenZeroOrNegative));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 0, limit: 0, searchField: "", searchQuery: "", ct: CancellationToken.None);

            result.Page.Should().Be(1);
            result.Limit.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.List.Should().HaveCount(2);
            result.List.Select(x => x.IdService).Should().Equal(1, 2);
        }

        [Fact]
        public async Task GetServices_ShouldFilterByTitle()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldFilterByTitle));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 1, limit: 10, searchField: "title", searchQuery: "spa", ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            result.List.First().IdService.Should().Be(1);
        }

        [Fact]
        public async Task GetServices_ShouldFilterByDescription()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldFilterByDescription));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 1, limit: 10, searchField: "description", searchQuery: "buff", ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            result.List.First().IdService.Should().Be(2);
        }

        [Fact]
        public async Task GetServices_ShouldFilterBySum_WhenParsable()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldFilterBySum_WhenParsable));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 1, limit: 10, searchField: "sum", searchQuery: "15", ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            result.List.First().IdService.Should().Be(2);
        }

        [Fact]
        public async Task GetServices_ShouldReturnEmpty_WhenSumNotParsable()
        {
            using var ctx = CreateCtx(nameof(GetServices_ShouldReturnEmpty_WhenSumNotParsable));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetServices(hotelId: 1, page: 1, limit: 10, searchField: "sum", searchQuery: "abc", ct: CancellationToken.None);

            result.TotalCount.Should().Be(0);
            result.List.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSpecificService_ShouldReturnOk()
        {
            using var ctx = CreateCtx(nameof(GetSpecificService_ShouldReturnOk));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var result = await sut.GetSpecificService(hotelId: 1, idService: 2, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.IdService.Should().Be(2);
            result.Data.Title.Should().Be("Breakfast");
            result.Data.Sum.Should().Be(15m);
            result.Data.Description.Should().Be("Buffet");
        }

        [Fact]
        public async Task GetSpecificService_ShouldReturnNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(GetSpecificService_ShouldReturnNotFound_WhenWrongHotelOrId));
            Seed(ctx);
            var sut = new ServiceQueryService(ctx, new Mock<ILogger<ServiceQueryService>>().Object);

            var r1 = await sut.GetSpecificService(hotelId: 1, idService: 3, ct: CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.GetSpecificService(hotelId: 1, idService: 999, ct: CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
