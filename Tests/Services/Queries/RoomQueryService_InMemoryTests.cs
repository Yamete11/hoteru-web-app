using System.Linq;
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
    public class RoomQueryService_InMemoryTests
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
            var ready = new RoomStatus { IdRoomStatus = 1, Title = "Ready" };
            var cleaning = new RoomStatus { IdRoomStatus = 2, Title = "Cleaning" };
            ctx.RoomStatuses.AddRange(ready, cleaning);

            var standard = new RoomType { IdRoomType = 1, Title = "Standard" };
            var deluxe = new RoomType { IdRoomType = 2, Title = "Deluxe" };
            ctx.RoomTypes.AddRange(standard, deluxe);

            var manager = new UserType { IdUserType = 1, Title = "Manager" };
            ctx.UserTypes.Add(manager);

            var p1 = new Person { IdPerson = 1, IdHotel = 1, Name = "U1", Surname = "S1", Email = "u1@x.com" };
            var p2 = new Person { IdPerson = 2, IdHotel = 2, Name = "U2", Surname = "S2", Email = "u2@x.com" };
            ctx.Persons.AddRange(p1, p2);

            var u1 = new User { IdPerson = 1, Person = p1, IdUserType = 1, UserType = manager, LoginName = "u1", Password = "pwd" };
            var u2 = new User { IdPerson = 2, Person = p2, IdUserType = 1, UserType = manager, LoginName = "u2", Password = "pwd" };
            ctx.Users.AddRange(u1, u2);

            var r1 = new Room { IdRoom = 1, Number = "101", Capacity = 2, Price = 100m, IdRoomStatus = ready.IdRoomStatus, RoomStatus = ready, IdRoomType = standard.IdRoomType, RoomType = standard, User = u1 };
            var r2 = new Room { IdRoom = 2, Number = "102", Capacity = 3, Price = 150m, IdRoomStatus = cleaning.IdRoomStatus, RoomStatus = cleaning, IdRoomType = deluxe.IdRoomType, RoomType = deluxe, User = u1 };
            var r3 = new Room { IdRoom = 3, Number = "201", Capacity = 4, Price = 200m, IdRoomStatus = ready.IdRoomStatus, RoomStatus = ready, IdRoomType = standard.IdRoomType, RoomType = standard, User = u2 };
            ctx.Rooms.AddRange(r1, r2, r3);

            ctx.SaveChanges();
        }


        [Fact]
        public async Task GetFreeRooms_ShouldReturnOnlyReadyForHotel_WhenIdRoomIsZero()
        {
            using var ctx = CreateCtx(nameof(GetFreeRooms_ShouldReturnOnlyReadyForHotel_WhenIdRoomIsZero));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetFreeRooms(hotelId: 1, idRoom: 0, ct: CancellationToken.None);

            result.Should().HaveCount(1);
            var r = result.First();
            r.IdRoom.Should().Be(1);
            r.Number.Should().Be("101");
            r.Status.Should().Be("Ready");
            r.Type.Should().Be("Standard");
        }

        [Fact]
        public async Task GetFreeRooms_ShouldIncludeRequestedRoom_EvenIfNotReady()
        {
            using var ctx = CreateCtx(nameof(GetFreeRooms_ShouldIncludeRequestedRoom_EvenIfNotReady));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetFreeRooms(hotelId: 1, idRoom: 2, ct: CancellationToken.None);

            result.Select(x => x.IdRoom).Should().Equal(1, 2);
            result.First().Status.Should().Be("Ready");
            result.Last().Status.Should().Be("Cleaning");
        }

        [Fact]
        public async Task GetRooms_ShouldPageAndMapWithoutFilter()
        {
            using var ctx = CreateCtx(nameof(GetRooms_ShouldPageAndMapWithoutFilter));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetRooms(hotelId: 1, page: 1, limit: 1, ct: CancellationToken.None);

            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.Limit.Should().Be(1);
            result.List.Should().HaveCount(1);
            var r = result.List.First();
            r.IdRoom.Should().Be(1);
            r.Number.Should().Be("101");
            r.Capacity.Should().Be(2);
            r.Price.Should().Be(100m);
            r.Status.Should().Be("Ready");
            r.Type.Should().Be("Standard");
        }

        [Fact]
        public async Task GetRooms_ShouldNormalizePageAndLimit()
        {
            using var ctx = CreateCtx(nameof(GetRooms_ShouldNormalizePageAndLimit));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetRooms(hotelId: 1, page: 0, limit: 0, ct: CancellationToken.None);

            result.Page.Should().Be(1);
            result.Limit.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.List.Select(x => x.IdRoom).Should().Equal(1, 2);
        }

        [Fact]
        public async Task GetRooms_ShouldFilterByCapacity_WhenSearchFieldCapacity()
        {
            using var ctx = CreateCtx(nameof(GetRooms_ShouldFilterByCapacity_WhenSearchFieldCapacity));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetRooms(hotelId: 1, page: 1, limit: 10, searchQuery: "3", searchField: "capacity", ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            result.List.First().IdRoom.Should().Be(2);
        }

        [Fact]
        public async Task GetSpecificRoom_ShouldReturnOk_WhenFound()
        {
            using var ctx = CreateCtx(nameof(GetSpecificRoom_ShouldReturnOk_WhenFound));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetSpecificRoom(hotelId: 1, idRoom: 2, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.IdRoom.Should().Be(2);
            result.Data.Number.Should().Be("102");
            result.Data.Capacity.Should().Be(3);
            result.Data.Price.Should().Be(150m);
            result.Data.Status.Should().Be(2);
            result.Data.Type.Should().Be(2);
        }

        [Fact]
        public async Task GetSpecificRoom_ShouldReturnNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(GetSpecificRoom_ShouldReturnNotFound_WhenMissing));
            Seed(ctx);
            var sut = new RoomQueryService(ctx, new Mock<ILogger<RoomQueryService>>().Object);

            var result = await sut.GetSpecificRoom(hotelId: 1, idRoom: 999, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be("Room not found");
            result.Data.Should().BeNull();
        }
    }
}
