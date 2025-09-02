using System;
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
    public class ReservationQueryService_InMemoryTests
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
            var rsReady = new RoomStatus { IdRoomStatus = 1, Title = "Ready" };
            var rsClean = new RoomStatus { IdRoomStatus = 2, Title = "Cleaning" };
            ctx.RoomStatuses.AddRange(rsReady, rsClean);

            var rtStd = new RoomType { IdRoomType = 1, Title = "Standard" };
            var rtDel = new RoomType { IdRoomType = 2, Title = "Deluxe" };
            ctx.RoomTypes.AddRange(rtStd, rtDel);

            var ut = new UserType { IdUserType = 1, Title = "Manager" };
            ctx.UserTypes.Add(ut);

            var pMgr1 = new Person { IdPerson = 10, IdHotel = 1, Name = "Mgr1", Surname = "S", Email = "m1@x.com" };
            var pMgr2 = new Person { IdPerson = 20, IdHotel = 2, Name = "Mgr2", Surname = "S", Email = "m2@x.com" };
            var pG1 = new Person { IdPerson = 1, IdHotel = 1, Name = "Ivan", Surname = "Ivanov", Email = "ivan@x.com" };
            var pG2 = new Person { IdPerson = 2, IdHotel = 1, Name = "Petr", Surname = "Petrov", Email = "petr@x.com" };
            var pG3 = new Person { IdPerson = 3, IdHotel = 2, Name = "Olga", Surname = "Sidorova", Email = "olga@x.com" };
            ctx.Persons.AddRange(pMgr1, pMgr2, pG1, pG2, pG3);

            var u1 = new User { IdPerson = 10, Person = pMgr1, IdUserType = 1, UserType = ut, LoginName = "mgr1", Password = "pwd" };
            var u2 = new User { IdPerson = 20, Person = pMgr2, IdUserType = 1, UserType = ut, LoginName = "mgr2", Password = "pwd" };
            ctx.Users.AddRange(u1, u2);

            var gsActive = new GuestStatus { IdGuestStatus = 1, Title = "Active" };
            ctx.GuestStatuses.Add(gsActive);

            var g1 = new Guest { IdPerson = 1, Person = pG1, IdGuestStatus = 1, GuestStatus = gsActive };
            var g2 = new Guest { IdPerson = 2, Person = pG2, IdGuestStatus = 1, GuestStatus = gsActive };
            var g3 = new Guest { IdPerson = 3, Person = pG3, IdGuestStatus = 1, GuestStatus = gsActive };
            ctx.Guests.AddRange(g1, g2, g3);

            var r1 = new Room { IdRoom = 101, Number = "101", Capacity = 2, Price = 100m, IdRoomStatus = 1, RoomStatus = rsReady, IdRoomType = 1, RoomType = rtStd, User = u1 };
            var r2 = new Room { IdRoom = 102, Number = "102", Capacity = 3, Price = 150m, IdRoomStatus = 1, RoomStatus = rsReady, IdRoomType = 2, RoomType = rtDel, User = u1 };
            var r3 = new Room { IdRoom = 201, Number = "201", Capacity = 4, Price = 200m, IdRoomStatus = 2, RoomStatus = rsClean, IdRoomType = 1, RoomType = rtStd, User = u2 };
            ctx.Rooms.AddRange(r1, r2, r3);

            var depType = new DepositType { IdDepositType = 1, Title = "Cash" };
            ctx.DepositTypes.Add(depType);
            var dep1 = new Deposit { IdDeposit = 1, Sum = 50m, IdDepositType = 1, DepositType = depType };
            ctx.Deposits.Add(dep1);

            var bill1 = new Bill { IdBill = 1, Sum = 500m, Created = new DateTime(2024, 12, 31) };
            ctx.Bills.Add(bill1);

            var res1 = new Reservation
            {
                IdReservation = 1,
                In = new DateTime(2025, 1, 1),
                Out = new DateTime(2025, 1, 5),
                Room = r1,
                IdRoom = r1.IdRoom,
                User = u1,
                Guest = g1,
                IdGuest = g1.IdPerson,
                Confirmed = true,
                Price = 100m,
                Capacity = 2
            };

            var res2 = new Reservation
            {
                IdReservation = 2,
                In = new DateTime(2025, 2, 1),
                Out = new DateTime(2025, 2, 3),
                Room = r2,
                IdRoom = r2.IdRoom,
                User = u1,
                Guest = g1,
                IdGuest = g1.IdPerson,
                Confirmed = true,
                Bill = bill1,
                Price = 150m,
                Capacity = 3
            };

            var res3 = new Reservation
            {
                IdReservation = 3,
                In = new DateTime(2025, 3, 1),
                Out = new DateTime(2025, 3, 4),
                Room = r2,
                IdRoom = r2.IdRoom,
                User = u1,
                Guest = g2,
                IdGuest = g2.IdPerson,
                Confirmed = false,
                IdDeposit = 1,
                Deposit = dep1,
                Price = 150m,
                Capacity = 3
            };

            var res4 = new Reservation
            {
                IdReservation = 4,
                In = new DateTime(2025, 4, 1),
                Out = new DateTime(2025, 4, 2),
                Room = r3,
                IdRoom = r3.IdRoom,
                User = u2,
                Guest = g3,
                IdGuest = g3.IdPerson,
                Confirmed = true,
                Price = 200m,
                Capacity = 4
            };

            ctx.Reservations.AddRange(res1, res2, res3, res4);

            var s1 = new Service { IdService = 1, Title = "Spa", Sum = 30m };
            var s2 = new Service { IdService = 2, Title = "Breakfast", Sum = 15m };
            ctx.Services.AddRange(s1, s2);

            var rs1 = new ReservationService { IdReservation = 2, IdService = 1, Date = new DateTime(2025, 2, 2), Service = s1 };
            var rs2 = new ReservationService { IdReservation = 3, IdService = 2, Date = new DateTime(2025, 3, 2), Service = s2 };
            ctx.ReservationServices.AddRange(rs1, rs2);

            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetReservations_ShouldReturnConfirmedWithoutBill_ForHotel()
        {
            using var ctx = CreateCtx(nameof(GetReservations_ShouldReturnConfirmedWithoutBill_ForHotel));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var result = await sut.GetReservations(hotelId: 1, page: 1, limit: 10, ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            var r = result.List.First();
            r.IdReservation.Should().Be(1);
            r.In.Should().Be("2025-01-01");
            r.Out.Should().Be("2025-01-05");
            r.RoomNumber.Should().Be("101");
            r.BookedBy.Should().Be("mgr1");
            r.Name.Should().Be("Ivan");
            r.Surname.Should().Be("Ivanov");
        }

        [Fact]
        public async Task GetHistory_ShouldReturnWithBill_ForHotel()
        {
            using var ctx = CreateCtx(nameof(GetHistory_ShouldReturnWithBill_ForHotel));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var result = await sut.GetHistory(hotelId: 1, page: 1, limit: 10, ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            var r = result.List.First();
            r.IdReservation.Should().Be(2);
            r.RoomNumber.Should().Be("102");
            r.BookedBy.Should().Be("mgr1");
            r.Name.Should().Be("Ivan");
            r.Surname.Should().Be("Ivanov");
        }

        [Fact]
        public async Task GetArrivals_ShouldReturnUnconfirmed_ForHotel()
        {
            using var ctx = CreateCtx(nameof(GetArrivals_ShouldReturnUnconfirmed_ForHotel));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var result = await sut.GetArrivals(hotelId: 1, page: 1, limit: 10, ct: CancellationToken.None);

            result.TotalCount.Should().Be(1);
            result.List.Should().ContainSingle();
            result.List.First().IdReservation.Should().Be(3);
        }

        [Fact]
        public async Task GetSpecificHistory_ShouldReturnOk()
        {
            using var ctx = CreateCtx(nameof(GetSpecificHistory_ShouldReturnOk));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var result = await sut.GetSpecificHistory(hotelId: 1, idReservation: 2, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            var d = result.Data!;
            d.IdReservation.Should().Be(2);
            d.In.Should().Be("2025-02-01");
            d.Out.Should().Be("2025-02-03");
            d.RoomNumber.Should().Be("102");
            d.RoomType.Should().Be("Deluxe");
            d.BookedBy.Should().Be("mgr1");
            d.Name.Should().Be("Ivan");
            d.Surname.Should().Be("Ivanov");
            d.DepositSum.Should().Be(0m);
            d.DepositType.Should().Be(string.Empty);
            d.BillSum.Should().Be(500m);
            d.Created.Should().Be("2024-12-31");
            d.Services.Should().HaveCount(1);
            var svc = d.Services.First();
            svc.IdService.Should().Be(1);
            svc.Title.Should().Be("Spa");
            svc.Sum.Should().Be(30m);
            svc.Date.Date.Should().Be(new DateTime(2025, 2, 2));
        }

        [Fact]
        public async Task GetSpecificHistory_ShouldReturnNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(GetSpecificHistory_ShouldReturnNotFound_WhenWrongHotelOrId));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var r1 = await sut.GetSpecificHistory(hotelId: 1, idReservation: 999, ct: CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.GetSpecificHistory(hotelId: 2, idReservation: 2, ct: CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetSpecificArrival_ShouldReturnOk()
        {
            using var ctx = CreateCtx(nameof(GetSpecificArrival_ShouldReturnOk));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var result = await sut.GetSpecificArrival(hotelId: 1, idArrival: 3, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            var a = result.Data!;
            a.IdReservation.Should().Be(3);
            a.In.Should().Be(new DateTime(2025, 3, 1));
            a.Out.Should().Be(new DateTime(2025, 3, 4));
            a.Capacity.Should().Be(3);
            a.IdRoom.Should().Be(102);
            a.IdDepositType.Should().Be(1);
            a.IdGuest.Should().Be(2);
            a.IdRoomType.Should().Be(2);
            a.Confirmed.Should().BeFalse();
            a.DepositSum.Should().Be(50m);
            a.Price.Should().Be(150m);
            a.Services.Should().HaveCount(1);
            var svc = a.Services.First();
            svc.IdService.Should().Be(2);
            svc.Title.Should().Be("Breakfast");
            svc.Sum.Should().Be(15m);
            svc.Date.Date.Should().Be(new DateTime(2025, 3, 2));
        }

        [Fact]
        public async Task GetSpecificArrival_ShouldReturnNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(GetSpecificArrival_ShouldReturnNotFound_WhenWrongHotelOrId));
            Seed(ctx);
            var sut = new ReservationQueryService(ctx, new Mock<ILogger<ReservationQueryService>>().Object);

            var r1 = await sut.GetSpecificArrival(hotelId: 1, idArrival: 999, ct: CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.GetSpecificArrival(hotelId: 2, idArrival: 3, ct: CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
