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
    public class RoomStatusServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_RoomStatus")
                .Options;

            var context = new MyDbContext(options);

            context.RoomStatuses.AddRange(
                new RoomStatus { IdRoomStatus = 1, Title = "Ready" },
                new RoomStatus { IdRoomStatus = 2, Title = "Out of service" },
                new RoomStatus { IdRoomStatus = 3, Title = "Occupied" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetRoomStatuses_ReturnsAllStatuses()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomStatusService(context);

            IEnumerable<StatusDTO> result = await service.GetRoomStatuses();

            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Ready", item.Title),
                item => Assert.Equal("Out of service", item.Title),
                item => Assert.Equal("Occupied", item.Title));
        }
    }

}
