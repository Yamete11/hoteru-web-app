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
    public class GuestStatusServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_GuestStatus")
                .Options;

            var context = new MyDbContext(options);

            context.GuestStatuses.AddRange(
                new GuestStatus { IdGuestStatus = 1, Title = "Regular" },
                new GuestStatus { IdGuestStatus = 2, Title = "VIP" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetGuestStatuses_ReturnsAllStatuses()
        {
            var context = GetInMemoryDbContext();
            var service = new GuestStatusService(context);

            IEnumerable<StatusDTO> result = await service.GetGuestStatuses();

            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Regular", item.Title),
                item => Assert.Equal("VIP", item.Title));
        }
    }

}
