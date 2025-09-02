using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace hoteru_be.Tests.Services.Commands
{
    public class RoomCommandService_InMemoryTests
    {
        private static MyDbContext CreateCtx(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new MyDbContext(options);
        }

        private static void SeedBasics(MyDbContext ctx)
        {
            var ready = new RoomStatus { IdRoomStatus = 1, Title = "Ready" };
            var occupied = new RoomStatus { IdRoomStatus = 2, Title = "Occupied" };
            ctx.RoomStatuses.AddRange(ready, occupied);

            var rtStd = new RoomType { IdRoomType = 1, Title = "Standard" };
            var rtDel = new RoomType { IdRoomType = 2, Title = "Deluxe" };
            ctx.RoomTypes.AddRange(rtStd, rtDel);

            var ut = new UserType { IdUserType = 1, Title = "Manager" };
            ctx.UserTypes.Add(ut);

            var h1 = new Hotel { IdHotel = 1, Title = "H1", Address = new Address { IdAddress = 1, City = "W", Country = "PL", Street = "S1", Postcode = "00-001" } };
            var h2 = new Hotel { IdHotel = 2, Title = "H2", Address = new Address { IdAddress = 2, City = "K", Country = "PL", Street = "S2", Postcode = "30-001" } };
            ctx.Hotels.AddRange(h1, h2);

            var p1 = new Person { IdPerson = 1, IdHotel = 1, Name = "U1", Surname = "S1", Email = "u1@x.com", Hotel = h1 };
            var p2 = new Person { IdPerson = 2, IdHotel = 1, Name = "U2", Surname = "S2", Email = "u2@x.com", Hotel = h1 };
            var p3 = new Person { IdPerson = 3, IdHotel = 2, Name = "U3", Surname = "S3", Email = "u3@x.com", Hotel = h2 };
            ctx.Persons.AddRange(p1, p2, p3);

            var u1 = new User { IdPerson = 1, Person = p1, IdUserType = 1, UserType = ut, LoginName = "u1", Password = "x" };
            var u2 = new User { IdPerson = 2, Person = p2, IdUserType = 1, UserType = ut, LoginName = "u2", Password = "x" };
            var u3 = new User { IdPerson = 3, Person = p3, IdUserType = 1, UserType = ut, LoginName = "u3", Password = "x" };
            ctx.Users.AddRange(u1, u2, u3);

            var r1 = new Room { IdRoom = 101, Number = "101", Capacity = 2, Price = 100m, IdRoomStatus = 1, RoomStatus = ready, IdRoomType = 1, RoomType = rtStd, User = u1 };
            var r2 = new Room { IdRoom = 102, Number = "102", Capacity = 3, Price = 150m, IdRoomStatus = 1, RoomStatus = ready, IdRoomType = 2, RoomType = rtDel, User = u1 };
            var r3 = new Room { IdRoom = 201, Number = "201", Capacity = 4, Price = 200m, IdRoomStatus = 2, RoomStatus = occupied, IdRoomType = 1, RoomType = rtStd, User = u3 };
            ctx.Rooms.AddRange(r1, r2, r3);

            ctx.SaveChanges();
        }

        private static RoomCommandService CreateSut(MyDbContext ctx)
        {
            var logger = new Mock<ILogger<RoomCommandService>>();
            return new RoomCommandService(ctx, logger.Object);
        }

        [Fact]
        public async Task PostRoom_ShouldCreate_WhenValid()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldCreate_WhenValid));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new RoomDTO { Number = "103", Capacity = 2, Price = 120m, Status = "1", Type = "2" };

            var res = await sut.PostRoom(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Rooms.Any(r => r.Number == "103" && r.User.Person.IdHotel == 1).Should().BeTrue();
        }

        [Fact]
        public async Task PostRoom_ShouldFail_WhenDuplicateNumberInHotel()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldFail_WhenDuplicateNumberInHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(1, new RoomDTO { Number = "101", Capacity = 2, Price = 120m, Status = "1", Type = "1" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("Room number already exists.");
            res.Errors.Should().ContainKey("Number");
        }

        [Fact]
        public async Task PostRoom_ShouldUnprocessable_WhenStatusNotNumeric()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldUnprocessable_WhenStatusNotNumeric));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(1, new RoomDTO { Number = "200", Capacity = 1, Price = 1m, Status = "x", Type = "1" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            res.Errors.Should().ContainKey("Status");
        }

        [Fact]
        public async Task PostRoom_ShouldUnprocessable_WhenTypeNotNumeric()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldUnprocessable_WhenTypeNotNumeric));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(1, new RoomDTO { Number = "200", Capacity = 1, Price = 1m, Status = "1", Type = "x" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            res.Errors.Should().ContainKey("Type");
        }

        [Fact]
        public async Task PostRoom_ShouldNotFound_WhenStatusMissing()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldNotFound_WhenStatusMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(1, new RoomDTO { Number = "200", Capacity = 1, Price = 1m, Status = "999", Type = "1" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("Room status not found");
        }

        [Fact]
        public async Task PostRoom_ShouldNotFound_WhenTypeMissing()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldNotFound_WhenTypeMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(1, new RoomDTO { Number = "200", Capacity = 1, Price = 1m, Status = "1", Type = "999" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("Room type not found");
        }

        [Fact]
        public async Task PostRoom_ShouldError_WhenNoUserForHotel()
        {
            using var ctx = CreateCtx(nameof(PostRoom_ShouldError_WhenNoUserForHotel));
            SeedBasics(ctx);
            ctx.Users.RemoveRange(ctx.Users.Where(u => u.Person.IdHotel == 2));
            ctx.SaveChanges();
            var sut = CreateSut(ctx);

            var res = await sut.PostRoom(2, new RoomDTO { Number = "300", Capacity = 1, Price = 1m, Status = "1", Type = "1" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            res.Message.Should().Be("No user found for this hotel");
        }

        [Fact]
        public async Task UpdateRoom_ShouldUpdate_WhenValid()
        {
            using var ctx = CreateCtx(nameof(UpdateRoom_ShouldUpdate_WhenValid));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new RoomDTO { IdRoom = 101, Number = "101A", Capacity = 5, Price = 222m, Status = "2", Type = "2" };

            var res = await sut.UpdateRoom(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var r = ctx.Rooms.Single(x => x.IdRoom == 101);
            r.Number.Should().Be("101A");
            r.Capacity.Should().Be(5);
            r.Price.Should().Be(222m);
            r.IdRoomStatus.Should().Be(2);
            r.IdRoomType.Should().Be(2);
        }

        [Fact]
        public async Task UpdateRoom_ShouldNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(UpdateRoom_ShouldNotFound_WhenWrongHotelOrId));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 999, Number = "X", Status = "1", Type = "1" }, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 201, Number = "X", Status = "1", Type = "1" }, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateRoom_ShouldFail_WhenDuplicateNumberInHotel()
        {
            using var ctx = CreateCtx(nameof(UpdateRoom_ShouldFail_WhenDuplicateNumberInHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 102, Number = "101", Status = "1", Type = "2" }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("Another room with this number already exists");
            res.Errors.Should().ContainKey("Number");
        }

        [Fact]
        public async Task UpdateRoom_ShouldUnprocessable_WhenStatusOrTypeNotNumeric()
        {
            using var ctx = CreateCtx(nameof(UpdateRoom_ShouldUnprocessable_WhenStatusOrTypeNotNumeric));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 101, Number = "101", Status = "x", Type = "1" }, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            r1.Errors.Should().ContainKey("Status");

            var r2 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 101, Number = "101", Status = "1", Type = "x" }, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            r2.Errors.Should().ContainKey("Type");
        }

        [Fact]
        public async Task UpdateRoom_ShouldNotFound_WhenStatusOrTypeMissing()
        {
            using var ctx = CreateCtx(nameof(UpdateRoom_ShouldNotFound_WhenStatusOrTypeMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 101, Number = "101", Status = "999", Type = "1" }, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            r1.Message.Should().Be("Room status not found");

            var r2 = await sut.UpdateRoom(1, new RoomDTO { IdRoom = 101, Number = "101", Status = "1", Type = "999" }, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            r2.Message.Should().Be("Room type not found");
        }

        [Fact]
        public async Task DeleteRoom_ShouldNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(DeleteRoom_ShouldNotFound_WhenWrongHotelOrId));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.DeleteRoom(1, idRoom: 999, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.DeleteRoom(1, idRoom: 201, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteRoom_ShouldConflict_WhenOccupied()
        {
            using var ctx = CreateCtx(nameof(DeleteRoom_ShouldConflict_WhenOccupied));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.DeleteRoom(2, idRoom: 201, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
            res.Message.Should().Be("You cannot delete an occupied room");
        }

       

        private sealed class ThrowingDbContext : MyDbContext
        {
            public bool ThrowOnSave { get; set; }
            public ThrowingDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                if (ThrowOnSave) throw new DbUpdateException("boom", innerException: null);
                return base.SaveChangesAsync(cancellationToken);
            }
        }

        [Fact]
        public async Task DeleteRoom_ShouldReturnConflict_WhenDbUpdateException()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(nameof(DeleteRoom_ShouldReturnConflict_WhenDbUpdateException))
                .Options;
            using var ctx = new ThrowingDbContext(options);
            SeedBasics(ctx);
            ctx.ThrowOnSave = false;

            var sut = CreateSut(ctx);

            var exists = ctx.Rooms.Single(r => r.IdRoom == 101);
            exists.Should().NotBeNull();

            ctx.ThrowOnSave = true;
            var res = await sut.DeleteRoom(1, idRoom: 101, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
            res.Message.Should().Be("Cannot delete room due to related data.");
        }
    }
}
