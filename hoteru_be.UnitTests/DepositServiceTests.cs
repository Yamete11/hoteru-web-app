using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace hoteru_be.UnitTests
{
    public class DepositServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new MyDbContext(options);

            context.Deposits.AddRange(
                new Deposit { IdDeposit = 1, Sum = 100, IdDepositType = 1 },
                new Deposit { IdDeposit = 2, Sum = 200, IdDepositType = 2 }
            );
            context.SaveChanges();

            return context;
        }


        [Fact]
        public async Task GetDeposit_ReturnsCorrectDeposit()
        {
            var context = GetInMemoryDbContext();
            var service = new DepositService(context);

            DepositDTO result = await service.GetDeposit(2);

            Assert.NotNull(result);
            Assert.Equal(2, result.IdDeposit);
            Assert.Equal(200, result.Sum);
            Assert.Equal(2, result.IdDepositType);
        }

        [Fact]
        public async Task GetDeposit_ReturnsNull_WhenNotFound()
        {
            var context = GetInMemoryDbContext();
            var service = new DepositService(context);

            var result = await service.GetDeposit(999);

            Assert.Null(result);
        }
    }

}
