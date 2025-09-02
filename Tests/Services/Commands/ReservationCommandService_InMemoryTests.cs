using System;
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
    public class ReservationCommandService_InMemoryTests
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
            var rs1 = new RoomStatus { IdRoomStatus = 1, Title = "Ready" };
            var rs2 = new RoomStatus { IdRoomStatus = 2, Title = "Occupied" };
            var rs3 = new RoomStatus { IdRoomStatus = 3, Title = "Available" };
            var rs4 = new RoomStatus { IdRoomStatus = 4, Title = "Out of service" };
            ctx.RoomStatuses.AddRange(rs1, rs2, rs3, rs4);

            var rt = new RoomType { IdRoomType = 1, Title = "Standard" };
            ctx.RoomTypes.Add(rt);

            var ut = new UserType { IdUserType = 1, Title = "Manager" };
            ctx.UserTypes.Add(ut);

            var h1 = new Hotel { IdHotel = 1, Title = "H1", Address = new Address { IdAddress = 1, City = "W", Country = "PL", Street = "S1", Postcode = "00-001" } };
            var h2 = new Hotel { IdHotel = 2, Title = "H2", Address = new Address { IdAddress = 2, City = "K", Country = "PL", Street = "S2", Postcode = "30-001" } };
            ctx.Hotels.AddRange(h1, h2);

            var pU1 = new Person { IdPerson = 100, IdHotel = 1, Name = "U1", Surname = "S1", Email = "u1@x.com", Hotel = h1 };
            var pU2 = new Person { IdPerson = 200, IdHotel = 2, Name = "U2", Surname = "S2", Email = "u2@x.com", Hotel = h2 };
            ctx.Persons.AddRange(pU1, pU2);

            var u1 = new User { IdPerson = 100, Person = pU1, IdUserType = 1, UserType = ut, LoginName = "mgr1", Password = "x" };
            var u2 = new User { IdPerson = 200, Person = pU2, IdUserType = 1, UserType = ut, LoginName = "mgr2", Password = "x" };
            ctx.Users.AddRange(u1, u2);

            var rFree = new Room { IdRoom = 101, Number = "101", Capacity = 2, Price = 100m, IdRoomStatus = 3, RoomStatus = rs3, IdRoomType = 1, RoomType = rt, User = u1 };
            var rReady = new Room { IdRoom = 102, Number = "102", Capacity = 2, Price = 120m, IdRoomStatus = 1, RoomStatus = rs1, IdRoomType = 1, RoomType = rt, User = u1 };
            var rOtherHotel = new Room { IdRoom = 201, Number = "201", Capacity = 2, Price = 150m, IdRoomStatus = 3, RoomStatus = rs3, IdRoomType = 1, RoomType = rt, User = u2 };
            ctx.Rooms.AddRange(rFree, rReady, rOtherHotel);

            var g1p = new Person { IdPerson = 1, IdHotel = 1, Name = "Ivan", Surname = "Ivanov", Email = "ivan@x.com", Hotel = h1 };
            var g2p = new Person { IdPerson = 2, IdHotel = 2, Name = "Olga", Surname = "Sidorova", Email = "olga@x.com", Hotel = h2 };
            ctx.Persons.AddRange(g1p, g2p);

            var gsActive = new GuestStatus { IdGuestStatus = 1, Title = "Active" };
            ctx.GuestStatuses.Add(gsActive);

            var g1 = new Guest { IdPerson = 1, Person = g1p, IdGuestStatus = 1, GuestStatus = gsActive };
            var g2 = new Guest { IdPerson = 2, Person = g2p, IdGuestStatus = 1, GuestStatus = gsActive };
            ctx.Guests.AddRange(g1, g2);

            var s1 = new Service { IdService = 1, Title = "Spa", Sum = 30m, User = u1 };
            var s2 = new Service { IdService = 2, Title = "Breakfast", Sum = 15m, User = u1 };
            ctx.Services.AddRange(s1, s2);

            ctx.DepositTypes.AddRange(
                new DepositType { IdDepositType = 5, Title = "Card5" },
                new DepositType { IdDepositType = 9, Title = "Type9" },
                new DepositType { IdDepositType = 10, Title = "Type10" }
            );

            ctx.SaveChanges();
        }



        private static ReservationCommandService CreateSut(MyDbContext ctx)
        {
            var logger = new Mock<ILogger<ReservationCommandService>>();
            return new ReservationCommandService(ctx, logger.Object);
        }

        [Fact]
        public async Task PostReservation_ShouldCreate_WithDeposit_AndServices()
        {
            using var ctx = CreateCtx(nameof(PostReservation_ShouldCreate_WithDeposit_AndServices));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var dto = new PostReservationDTO
            {
                IdRoom = 101,
                IdPerson = 1,
                IdUser = 100,
                Capacity = 2,
                Price = 200m,
                In = new DateTime(2025, 1, 10),
                Out = new DateTime(2025, 1, 12),
                Confirmed = false,
                IdDepositType = 5,
                Sum = 50m,
                Services = new()
        {
            new ServiceHistoryDTO { IdService = 1 },
            new ServiceHistoryDTO { IdService = 2 }
        }
            };

            var res = await sut.PostReservation(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            var created = ctx.Reservations.Include(r => r.Deposit).Single(r => r.IdRoom == 101 && r.IdGuest == 1);
            created.Deposit.Should().NotBeNull();
            created.Deposit!.Sum.Should().Be(50m);
            created.Deposit!.IdDepositType.Should().Be(5);
            ctx.ReservationServices.Count(rs => rs.IdReservation == created.IdReservation).Should().Be(2);
            ctx.Rooms.Single(r => r.IdRoom == 101).IdRoomStatus.Should().Be(2);
        }


        [Fact]
        public async Task PostReservation_ShouldNotFound_WhenRoomMissingOrWrongHotel()
        {
            using var ctx = CreateCtx(nameof(PostReservation_ShouldNotFound_WhenRoomMissingOrWrongHotel));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.PostReservation(1, new PostReservationDTO { IdRoom = 999, IdPerson = 1, IdUser = 100 }, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            r1.Message.Should().Be("Room not found");

            var r2 = await sut.PostReservation(1, new PostReservationDTO { IdRoom = 201, IdPerson = 1, IdUser = 100 }, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            r2.Message.Should().Be("Room not found");
        }

        [Fact]
        public async Task PostReservation_ShouldBadRequest_WhenRoomNotFree()
        {
            using var ctx = CreateCtx(nameof(PostReservation_ShouldBadRequest_WhenRoomNotFree));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostReservation(1, new PostReservationDTO { IdRoom = 102, IdPerson = 1, IdUser = 100 }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("Room is occupied");
        }

        [Fact]
        public async Task PostReservation_ShouldNotFound_WhenGuestMissing()
        {
            using var ctx = CreateCtx(nameof(PostReservation_ShouldNotFound_WhenGuestMissing));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostReservation(1, new PostReservationDTO { IdRoom = 101, IdPerson = 999, IdUser = 100 }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("Guest not found");
        }

        [Fact]
        public async Task UpdateReservation_ShouldAddDeposit_AndSyncServices()
        {
            using var ctx = CreateCtx(nameof(UpdateReservation_ShouldAddDeposit_AndSyncServices));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var baseRes = new Reservation
            {
                IdReservation = 1,
                IdRoom = 101,
                IdUser = 100,
                IdGuest = 1,
                In = new DateTime(2025, 2, 1),
                Out = new DateTime(2025, 2, 3),
                Capacity = 2,
                Price = 100m,
                Confirmed = false
            };
            ctx.Reservations.Add(baseRes);
            ctx.ReservationServices.Add(new ReservationService { IdReservation = 1, IdService = 1, Date = new DateTime(2025, 2, 2) });
            ctx.SaveChanges();

            var dto = new ArrivalDTO
            {
                IdReservation = 1,
                IdGuest = 1,
                In = new DateTime(2025, 2, 2),
                Out = new DateTime(2025, 2, 4),
                Capacity = 3,
                IdRoom = 101,
                Price = 120m,
                IdDepositType = 10,
                DepositSum = 80m,
                Services = new[] { new ServiceHistoryDTO { IdService = 2 } }.ToList()
            };

            var res = await sut.UpdateReservation(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var updated = ctx.Reservations.Include(r => r.Deposit).Single(r => r.IdReservation == 1);
            updated.Deposit.Should().NotBeNull();
            updated.Deposit!.IdDepositType.Should().Be(10);
            updated.Deposit!.Sum.Should().Be(80m);
            ctx.ReservationServices.Where(x => x.IdReservation == 1).Select(x => x.IdService).Should().Equal(2);
            updated.In.Should().Be(new DateTime(2025, 2, 2));
            updated.Out.Should().Be(new DateTime(2025, 2, 4));
            updated.Capacity.Should().Be(3);
            updated.Price.Should().Be(120m);
        }

        [Fact]
        public async Task UpdateReservation_ShouldRemoveDeposit_WhenTypeBecomesZero()
        {
            using var ctx = CreateCtx(nameof(UpdateReservation_ShouldRemoveDeposit_WhenTypeBecomesZero));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var resv = new Reservation { IdReservation = 2, IdRoom = 101, IdUser = 100, IdGuest = 1, In = DateTime.Today, Out = DateTime.Today.AddDays(1), Capacity = 1, Price = 10m, Deposit = new Deposit { IdDeposit = 77, IdDepositType = 5, Sum = 50m } };
            ctx.Reservations.Add(resv);
            ctx.SaveChanges();

            var dto = new ArrivalDTO { IdReservation = 2, IdGuest = 1, In = resv.In, Out = resv.Out, Capacity = 1, IdRoom = 101, Price = 10m, IdDepositType = 0, DepositSum = 0, Services = Array.Empty<ServiceHistoryDTO>().ToList() };

            var res = await sut.UpdateReservation(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            ctx.Reservations.Include(r => r.Deposit).Single(r => r.IdReservation == 2).Deposit.Should().BeNull();
        }

        [Fact]
        public async Task UpdateReservation_ShouldUpdateDeposit_WhenTypeNonZero()
        {
            using var ctx = CreateCtx(nameof(UpdateReservation_ShouldUpdateDeposit_WhenTypeNonZero));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var resv = new Reservation { IdReservation = 3, IdRoom = 101, IdUser = 100, IdGuest = 1, In = DateTime.Today, Out = DateTime.Today.AddDays(1), Capacity = 1, Price = 10m, Deposit = new Deposit { IdDeposit = 88, IdDepositType = 2, Sum = 10m } };
            ctx.Reservations.Add(resv);
            ctx.SaveChanges();

            var dto = new ArrivalDTO { IdReservation = 3, IdGuest = 1, In = resv.In, Out = resv.Out, Capacity = 1, IdRoom = 101, Price = 10m, IdDepositType = 9, DepositSum = 99m, Services = Array.Empty<ServiceHistoryDTO>().ToList() };

            var res = await sut.UpdateReservation(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var upd = ctx.Reservations.Include(r => r.Deposit).Single(r => r.IdReservation == 3);
            upd.Deposit!.IdDepositType.Should().Be(9);
            upd.Deposit!.Sum.Should().Be(99m);
        }

        [Fact]
        public async Task UpdateReservation_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(UpdateReservation_ShouldNotFound_WhenMissing));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var dto = new ArrivalDTO { IdReservation = 999, IdGuest = 1, In = DateTime.Today, Out = DateTime.Today.AddDays(1), Capacity = 1, IdRoom = 101, Price = 10m, IdDepositType = 0, DepositSum = 0, Services = Array.Empty<ServiceHistoryDTO>().ToList() };

            var res = await sut.UpdateReservation(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteSpecificReservation_ShouldDelete_AndResetRoomStatus()
        {
            using var ctx = CreateCtx(nameof(DeleteSpecificReservation_ShouldDelete_AndResetRoomStatus));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var resv = new Reservation { IdReservation = 10, IdRoom = 101, IdUser = 100, IdGuest = 1, In = DateTime.Today, Out = DateTime.Today.AddDays(1), Capacity = 1, Price = 10m, Confirmed = false };
            ctx.Reservations.Add(resv);
            ctx.ReservationServices.Add(new ReservationService { IdReservation = 10, IdService = 1, Date = DateTime.Today });
            ctx.Rooms.Single(r => r.IdRoom == 101).IdRoomStatus = 2;
            ctx.SaveChanges();

            var res = await sut.DeleteSpecificReservation(1, 10, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            res.Message.Should().Be("Reservation deleted successfully");
            ctx.Reservations.Any(r => r.IdReservation == 10).Should().BeFalse();
            ctx.ReservationServices.Any(x => x.IdReservation == 10).Should().BeFalse();
            ctx.Rooms.Single(r => r.IdRoom == 101).IdRoomStatus.Should().Be(1);
        }

        [Fact]
        public async Task DeleteSpecificReservation_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(DeleteSpecificReservation_ShouldNotFound_WhenMissing));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.DeleteSpecificReservation(1, 999, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("Reservation not found");
        }

        [Fact]
        public async Task ConfirmReservation_ShouldConfirm_WhenNotConfirmed()
        {
            using var ctx = CreateCtx(nameof(ConfirmReservation_ShouldConfirm_WhenNotConfirmed));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var resv = new Reservation
            {
                IdReservation = 50,
                IdRoom = 101,
                Room = ctx.Rooms.Single(r => r.IdRoom == 101),
                IdUser = 100,
                User = ctx.Users.Single(u => u.IdPerson == 100),
                IdGuest = 1,
                Guest = ctx.Guests.Include(g => g.Person).Single(g => g.IdPerson == 1),
                In = new DateTime(2025, 3, 1),
                Out = new DateTime(2025, 3, 3),
                Capacity = 2,
                Price = 200m,
                Confirmed = false
            };
            ctx.Reservations.Add(resv);
            ctx.SaveChanges();

            var res = await sut.ConfirmReservation(1, 50, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            res.Message.Should().Be("Confirmed");
            var updated = ctx.Reservations.Include(r => r.Room).Single(r => r.IdReservation == 50);
            updated.Confirmed.Should().BeTrue();
            updated.Room.IdRoomStatus.Should().Be(2);
        }

        [Fact]
        public async Task ConfirmReservation_ShouldCloseAndCreateBill_WhenAlreadyConfirmed()
        {
            using var ctx = CreateCtx(nameof(ConfirmReservation_ShouldCloseAndCreateBill_WhenAlreadyConfirmed));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var resv = new Reservation
            {
                IdReservation = 60,
                IdRoom = 101,
                Room = ctx.Rooms.Single(r => r.IdRoom == 101),
                IdUser = 100,
                User = ctx.Users.Single(u => u.IdPerson == 100),
                IdGuest = 1,
                Guest = ctx.Guests.Include(g => g.Person).Single(g => g.IdPerson == 1),
                In = new DateTime(2025, 4, 1),
                Out = new DateTime(2025, 4, 2),
                Capacity = 2,
                Price = 250m,
                Confirmed = true
            };
            ctx.Reservations.Add(resv);
            ctx.SaveChanges();

            var res = await sut.ConfirmReservation(1, 60, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            res.Message.Should().Be("Confirmed");
            var updated = ctx.Reservations.Include(r => r.Room).Include(r => r.Bill).Single(r => r.IdReservation == 60);
            updated.Bill.Should().NotBeNull();
            updated.Bill!.Sum.Should().Be(250m);
            updated.Bill!.GuestName.Should().Be("Ivan");
            updated.Bill!.GuestSurname.Should().Be("Ivanov");
            updated.Bill!.RoomNumber.Should().Be("101");
            updated.Bill!.BookedBy.Should().Be("mgr1");
            updated.Room.IdRoomStatus.Should().Be(4);
        }

        [Fact]
        public async Task ConfirmReservation_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(ConfirmReservation_ShouldNotFound_WhenMissing));
            Seed(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.ConfirmReservation(1, 999, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("Reservation not found");
        }
    }
}
