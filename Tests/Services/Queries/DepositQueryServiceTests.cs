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
    public class DepositQueryService_InMemoryTests
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
            ctx.Deposits.AddRange(
                new Deposit { IdDeposit = 1, Sum = 100m, IdDepositType = 2 },
                new Deposit { IdDeposit = 2, Sum = 250m, IdDepositType = 3 }
            );
            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetDeposit_ShouldReturnOk_WhenDepositExists()
        {
            using var ctx = CreateCtx(nameof(GetDeposit_ShouldReturnOk_WhenDepositExists));
            Seed(ctx);

            var logger = new Mock<ILogger<DepositQueryService>>();
            var sut = new DepositQueryService(ctx, logger.Object);

            var result = await sut.GetDeposit(2, CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.IdDeposit.Should().Be(2);
            result.Data.Sum.Should().Be(250m);
            result.Data.IdDepositType.Should().Be(3);
        }

        [Fact]
        public async Task GetDeposit_ShouldReturnNotFound_WhenDepositDoesNotExist()
        {
            using var ctx = CreateCtx(nameof(GetDeposit_ShouldReturnNotFound_WhenDepositDoesNotExist));
            Seed(ctx);

            var logger = new Mock<ILogger<DepositQueryService>>();
            var sut = new DepositQueryService(ctx, logger.Object);

            var result = await sut.GetDeposit(999, CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be("Deposit not found");
            result.Data.Should().BeNull();
        }
    }
}
