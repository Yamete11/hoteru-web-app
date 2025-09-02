using System.Threading;
using System.Threading.Tasks;
using System.Net;
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
    public class HotelQueryService_InMemoryTests
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
            var addr = new Address { IdAddress = 1, City = "Warsaw", Country = "Poland", Street = "Main 1", Postcode = "00-001" };
            var hotel = new Hotel { IdHotel = 1, Title = "Hotel A", Address = addr };
            ctx.Addresses.Add(addr);
            ctx.Hotels.Add(hotel);
            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetHotel_ShouldReturnOk_WhenExists()
        {
            using var ctx = CreateCtx(nameof(GetHotel_ShouldReturnOk_WhenExists));
            Seed(ctx);
            var sut = new HotelQueryService(ctx, new Mock<ILogger<HotelQueryService>>().Object);

            var result = await sut.GetHotel(1, CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be("Hotel A");
            result.Data.City.Should().Be("Warsaw");
            result.Data.Country.Should().Be("Poland");
            result.Data.Street.Should().Be("Main 1");
            result.Data.Postcode.Should().Be("00-001");
        }

        [Fact]
        public async Task GetHotel_ShouldReturnNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(GetHotel_ShouldReturnNotFound_WhenMissing));
            Seed(ctx);
            var sut = new HotelQueryService(ctx, new Mock<ILogger<HotelQueryService>>().Object);

            var result = await sut.GetHotel(999, CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be("Hotel not found");
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetHotel_ShouldReturnError_WhenCanceled()
        {
            using var ctx = CreateCtx(nameof(GetHotel_ShouldReturnError_WhenCanceled));
            Seed(ctx);
            var sut = new HotelQueryService(ctx, new Mock<ILogger<HotelQueryService>>().Object);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var result = await sut.GetHotel(1, cts.Token);

            result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be("Unexpected error");
            result.Data.Should().BeNull();
        }
    }
}
