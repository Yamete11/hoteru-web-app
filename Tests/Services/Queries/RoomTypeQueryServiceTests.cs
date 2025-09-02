using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.Entities;
using hoteru_be.Services.Queries;
using hoteru_be.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Queries
{
    public class RoomTypeQueryServiceTests
    {
        [Fact]
        public async Task GetRoomTypes_ShouldReturnOk_WithMappedList()
        {
            var data = new List<RoomType>
            {
                new RoomType { IdRoomType = 1, Title = "Single" },
                new RoomType { IdRoomType = 2, Title = "Double" }
            }.AsQueryable();

            var set = DbSetMock.Create(data);

            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.RoomTypes).Returns(set.Object);

            var logger = new Mock<ILogger<RoomTypeQueryService>>();
            var sut = new RoomTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetRoomTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.HaveCount(2);

            result.Data[0].IdType.Should().Be(1);
            result.Data[0].Title.Should().Be("Single");
            result.Data[1].IdType.Should().Be(2);
            result.Data[1].Title.Should().Be("Double");

            logger.VerifyLogLevel(LogLevel.Information, Times.Once());
        }

        [Fact]
        public async Task GetRoomTypes_ShouldReturnOk_WhenEmpty()
        {
            var data = new List<RoomType>().AsQueryable();
            var set = DbSetMock.Create(data);

            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.RoomTypes).Returns(set.Object);

            var logger = new Mock<ILogger<RoomTypeQueryService>>();
            var sut = new RoomTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetRoomTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.BeEmpty();

            logger.VerifyLogLevel(LogLevel.Information, Times.Once());
        }
    }
}
