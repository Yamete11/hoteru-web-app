using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.Entities;
using hoteru_be.Services.Queries;
using hoteru_be.Tests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace hoteru_be.Tests.Services.Queries
{
    public class UserTypeQueryServiceTests
    {
        [Fact]
        public async Task GetUserTypes_ShouldReturnOk_WithMappedList()
        {
            var data = new List<UserType>
            {
                new UserType { IdUserType = 1, Title = "Admin" },
                new UserType { IdUserType = 2, Title = "Guest" }
            }.AsQueryable();

            var userTypesSet = DbSetMock.Create(data);

            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.UserTypes).Returns(userTypesSet.Object);

            var logger = new Mock<ILogger<UserTypeQueryService>>();
            var sut = new UserTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetUserTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.HaveCount(2);

            result.Data[0].IdType.Should().Be(1);
            result.Data[0].Title.Should().Be("Admin");
            result.Data[1].IdType.Should().Be(2);
            result.Data[1].Title.Should().Be("Guest");

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 2 user types")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetUserTypes_ShouldReturnOk_WhenEmpty()
        {
            var data = new List<UserType>().AsQueryable();
            var userTypesSet = DbSetMock.Create(data);

            var ctx = new Mock<MyDbContext>();
            ctx.Setup(c => c.UserTypes).Returns(userTypesSet.Object);

            var logger = new Mock<ILogger<UserTypeQueryService>>();
            var sut = new UserTypeQueryService(ctx.Object, logger.Object);

            var result = await sut.GetUserTypes(CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.BeEmpty();

            logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Fetched 0 user types")),
                    It.IsAny<System.Exception>(),
                    It.IsAny<System.Func<It.IsAnyType, System.Exception, string>>()),
                Times.Once);
        }
    }
}
