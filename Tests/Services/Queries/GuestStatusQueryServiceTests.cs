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
    public class GuestStatusQueryServiceTests
    {
        [Fact]
        public async Task GetGuestStatuses_ShouldReturnOk_WithMappedList()
        {
            var data = new List<GuestStatus>
            {
                new GuestStatus { IdGuestStatus = 1, Title = "Active" },
                new GuestStatus { IdGuestStatus = 2, Title = "Blocked" }
            }.AsQueryable();

            var set = DbSetMock.Create(data);
            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.GuestStatuses).Returns(set.Object);

            var logger = new Mock<ILogger<GuestStatusQueryService>>();
            var sut = new GuestStatusQueryService(ctx.Object, logger.Object);

            var result = await sut.GetGuestStatuses(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.HaveCount(2);
            result.Data[0].IdStatus.Should().Be(1);
            result.Data[0].Title.Should().Be("Active");
            result.Data[1].IdStatus.Should().Be(2);
            result.Data[1].Title.Should().Be("Blocked");

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 2 guest statuses")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetGuestStatuses_ShouldReturnOk_WhenEmpty()
        {
            var data = new List<GuestStatus>().AsQueryable();
            var set = DbSetMock.Create(data);
            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.GuestStatuses).Returns(set.Object);

            var logger = new Mock<ILogger<GuestStatusQueryService>>();
            var sut = new GuestStatusQueryService(ctx.Object, logger.Object);

            var result = await sut.GetGuestStatuses(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.BeEmpty();

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 0 guest statuses")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }
    }
}
