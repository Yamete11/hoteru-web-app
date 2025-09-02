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
    public class DepositTypeQueryServiceTests
    {
        [Fact]
        public async Task GetDepositTypes_ShouldReturnOk_WithMappedList()
        {
            var data = new List<DepositType>
            {
                new DepositType { IdDepositType = 1, Title = "Cash" },
                new DepositType { IdDepositType = 2, Title = "Card" }
            }.AsQueryable();

            var set = DbSetMock.Create(data);
            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.DepositTypes).Returns(set.Object);

            var logger = new Mock<ILogger<DepositTypeQueryService>>();
            var sut = new DepositTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetDepositTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.HaveCount(2);
            result.Data[0].IdType.Should().Be(1);
            result.Data[0].Title.Should().Be("Cash");
            result.Data[1].IdType.Should().Be(2);
            result.Data[1].Title.Should().Be("Card");

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 2 deposit types")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetDepositTypes_ShouldReturnOk_WhenEmpty()
        {
            var data = new List<DepositType>().AsQueryable();
            var set = DbSetMock.Create(data);
            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.DepositTypes).Returns(set.Object);

            var logger = new Mock<ILogger<DepositTypeQueryService>>();
            var sut = new DepositTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetDepositTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.BeEmpty();

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 0 deposit types")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }
    }
}
