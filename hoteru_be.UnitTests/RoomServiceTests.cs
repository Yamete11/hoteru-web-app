using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace hoteru_be.UnitTests
{
    public class RoomServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new MyDbContext(options);

            context.RoomStatuses.AddRange(
                new RoomStatus { IdRoomStatus = 1, Title = "Free" },
                new RoomStatus { IdRoomStatus = 2, Title = "Occupied" },
                new RoomStatus { IdRoomStatus = 3, Title = "Available" }
            );

            context.RoomTypes.Add(new RoomType { IdRoomType = 1, Title = "Standard" });

            context.Rooms.AddRange(
                new Room { IdRoom = 1, Number = "101", Capacity = 2, Price = 100, IdRoomStatus = 1, IdRoomType = 1 },
                new Room { IdRoom = 2, Number = "102", Capacity = 3, Price = 150, IdRoomStatus = 2, IdRoomType = 1 },
                new Room { IdRoom = 3, Number = "103", Capacity = 2, Price = 120, IdRoomStatus = 3, IdRoomType = 1 }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotFound_WhenRoomDoesNotExist()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.DeleteRoom(999);

            Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        }

        [Fact]
        public async Task DeleteRoom_ReturnsNotAcceptable_WhenRoomOccupied()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.DeleteRoom(2);

            Assert.Equal(HttpStatusCode.NotAcceptable, result.HttpStatusCode);
        }

        [Fact]
        public async Task DeleteRoom_DeletesRoom_WhenRoomFree()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.DeleteRoom(1);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.Null(await context.Rooms.FindAsync(1));
        }

        [Fact]
        public async Task PostRoom_ReturnsBadRequest_WhenNumberExists()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var dto = new RoomDTO
            {
                Number = "101",
                Capacity = 2,
                Price = 100,
                Type = "1",
                Status = "1"
            };

            var result = await service.PostRoom(dto);

            Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
        }

        [Fact]
        public async Task PostRoom_CreatesRoom_WhenValid()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var dto = new RoomDTO
            {
                Number = "104",
                Capacity = 2,
                Price = 120,
                Type = "1",
                Status = "1"
            };

            var result = await service.PostRoom(dto);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.NotNull(await context.Rooms.FirstOrDefaultAsync(r => r.Number == "104"));
        }

        [Fact]
        public async Task GetFreeRooms_ReturnsOnlyFreeAndSpecificRoom()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.GetFreeRooms(2);

            Assert.Contains(result, r => r.IdRoom == 2);
            Assert.Contains(result, r => r.Status == "Available");
        }

        [Fact]
        public async Task GetSpecificRoom_ReturnsCorrectRoom()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.GetSpecificRoom(1);

            Assert.Equal("101", result.Number);
            Assert.Equal(1, result.Status);
        }

        [Fact]
        public async Task UpdateRoom_ReturnsNotFound_WhenRoomMissing()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.UpdateRoom(new RoomDTO { IdRoom = 999, Number = "999", Capacity = 1, Price = 1, Status = "1", Type = "1" });

            Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        }

        [Fact]
        public async Task UpdateRoom_UpdatesRoom_WhenValid()
        {
            var context = GetInMemoryDbContext();
            var service = new RoomService(context);

            var result = await service.UpdateRoom(new RoomDTO { IdRoom = 1, Number = "105", Capacity = 4, Price = 300, Status = "1", Type = "1" });

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            var updated = await context.Rooms.FindAsync(1);
            Assert.Equal("105", updated.Number);
            Assert.Equal(300, updated.Price);
        }
    }

}
