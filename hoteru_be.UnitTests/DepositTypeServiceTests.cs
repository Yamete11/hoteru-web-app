using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace hoteru_be.UnitTests
{

    public class DepositTypeServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DepositType")
                .Options;

            var context = new MyDbContext(options);

            context.DepositTypes.AddRange(
                new DepositType { IdDepositType = 1, Title = "Cash" },
                new DepositType { IdDepositType = 2, Title = "Card" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetDepositTypes_ReturnsAllDepositTypes()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new DepositTypeService(context);

            // Act
            IEnumerable<TypeDTO> result = await service.GetDepositTypes();

            // Assert
            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Cash", item.Title),
                item => Assert.Equal("Card", item.Title));
        }
    }

}
