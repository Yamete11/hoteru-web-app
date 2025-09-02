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
    public class GuestCommandService_InMemoryTests
    {
        private static MyDbContext CreateCtx(string dbName)
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new MyDbContext(options);
        }

        private static void SeedBase(MyDbContext ctx)
        {
            var h1 = new Hotel { IdHotel = 1, Title = "H1", Address = new Address { IdAddress = 1, City = "C1", Country = "PL", Street = "S1", Postcode = "00-001" } };
            var h2 = new Hotel { IdHotel = 2, Title = "H2", Address = new Address { IdAddress = 2, City = "C2", Country = "PL", Street = "S2", Postcode = "00-002" } };
            ctx.Hotels.AddRange(h1, h2);

            ctx.GuestStatuses.AddRange(
                new GuestStatus { IdGuestStatus = 1, Title = "Active" },
                new GuestStatus { IdGuestStatus = 2, Title = "Blocked" }
            );

            ctx.SaveChanges();
        }

        private static Guest AddGuest(MyDbContext ctx, int personId, int hotelId, string name, string email, string tel, string passport, int statusId = 1)
        {
            var p = new Person { IdPerson = personId, IdHotel = hotelId, Name = name, Surname = "S", Email = email };
            var g = new Guest { IdPerson = personId, Person = p, TelNumber = tel, Passport = passport, IdGuestStatus = statusId };
            ctx.Persons.Add(p);
            ctx.Guests.Add(g);
            ctx.SaveChanges();
            return g;
        }

        private static GuestCommandService CreateSut(MyDbContext ctx)
        {
            var logger = new Mock<ILogger<GuestCommandService>>();
            return new GuestCommandService(ctx, logger.Object);
        }

        [Fact]
        public async Task PostGuest_ShouldCreate_WhenValidUnique()
        {
            using var ctx = CreateCtx(nameof(PostGuest_ShouldCreate_WhenValidUnique));
            SeedBase(ctx);
            AddGuest(ctx, 10, 2, "Other", "other@x.com", "999", "PP9");

            var sut = CreateSut(ctx);

            var dto = new GuestDTO
            {
                Name = "Ivan",
                Surname = "Ivanov",
                Email = "IVAN@x.com",
                TelNumber = "123",
                Passport = "P1",
                IdGuestStatus = "1"
            };

            var r = await sut.PostGuest(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Guests.Count().Should().Be(2);
            var created = ctx.Guests.Include(g => g.Person).Single(g => g.Person.Email == "ivan@x.com");
            created.TelNumber.Should().Be("123");
            created.Passport.Should().Be("P1");
            created.IdGuestStatus.Should().Be(1);
            created.Person.IdHotel.Should().Be(1);
        }

        [Fact]
        public async Task PostGuest_ShouldUnprocessable_OnDuplicates_AndInvalidStatus()
        {
            using var ctx = CreateCtx(nameof(PostGuest_ShouldUnprocessable_OnDuplicates_AndInvalidStatus));
            SeedBase(ctx);
            AddGuest(ctx, 5, 1, "A", "dup@x.com", "111", "PP1");
            AddGuest(ctx, 6, 2, "B", "other@x.com", "222", "PP2");

            var sut = CreateSut(ctx);

            var dto = new GuestDTO
            {
                Name = "X",
                Surname = "Y",
                Email = "DUP@x.com",
                TelNumber = "111",
                Passport = "PP1",
                IdGuestStatus = "999"
            };

            var r = await sut.PostGuest(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be((HttpStatusCode)422);
            r.Errors.Should().NotBeNull();
            r.Errors.Should().ContainKeys("Email", "TelNumber", "Passport", "IdGuestStatus");

        }

        [Fact]
        public async Task UpdateGuest_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(UpdateGuest_ShouldNotFound_WhenMissing));
            SeedBase(ctx);
            var sut = CreateSut(ctx);

            var dto = new GuestDTO
            {
                IdPerson = 999,
                Name = "N",
                Surname = "S",
                Email = "n@x.com",
                TelNumber = "1",
                Passport = "P",
                IdGuestStatus = "1"
            };

            var r = await sut.UpdateGuest(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateGuest_ShouldUnprocessable_OnConflicts_AndInvalidStatus()
        {
            using var ctx = CreateCtx(nameof(UpdateGuest_ShouldUnprocessable_OnConflicts_AndInvalidStatus));
            SeedBase(ctx);
            AddGuest(ctx, 1, 1, "A", "a@x.com", "111", "P1");
            AddGuest(ctx, 2, 1, "B", "b@x.com", "222", "P2");

            var sut = CreateSut(ctx);

            var dto = new GuestDTO
            {
                IdPerson = 1,
                Name = "A2",
                Surname = "S2",
                Email = "b@x.com",
                TelNumber = "222",
                Passport = "P2",
                IdGuestStatus = "0"
            };

            var r = await sut.UpdateGuest(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be((HttpStatusCode)422);
            r.Errors.Should().NotBeNull();
            r.Errors.Should().ContainKeys("Email", "TelNumber", "Passport", "IdGuestStatus");
        }

        [Fact]
        public async Task UpdateGuest_ShouldOk_UpdateFields()
        {
            using var ctx = CreateCtx(nameof(UpdateGuest_ShouldOk_UpdateFields));
            SeedBase(ctx);
            AddGuest(ctx, 1, 1, "Old", "old@x.com", "111", "P1");

            var sut = CreateSut(ctx);

            var dto = new GuestDTO
            {
                IdPerson = 1,
                Name = "New",
                Surname = "Surname",
                Email = "NEW@x.com",
                TelNumber = "999",
                Passport = "PP9",
                IdGuestStatus = "2"
            };

            var r = await sut.UpdateGuest(1, dto, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var g = ctx.Guests.Include(x => x.Person).Single(x => x.IdPerson == 1);
            g.TelNumber.Should().Be("999");
            g.Passport.Should().Be("PP9");
            g.IdGuestStatus.Should().Be(2);
            g.Person.Name.Should().Be("New");
            g.Person.Email.Should().Be("new@x.com");
        }

        [Fact]
        public async Task DeleteGuest_ShouldNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(DeleteGuest_ShouldNotFound_WhenMissing));
            SeedBase(ctx);
            var sut = CreateSut(ctx);

            var r = await sut.DeleteGuest(1, 999, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteGuest_ShouldConflict_WhenHasReservations()
        {
            using var ctx = CreateCtx(nameof(DeleteGuest_ShouldConflict_WhenHasReservations));
            SeedBase(ctx);
            AddGuest(ctx, 1, 1, "A", "a@x.com", "111", "P1");
            ctx.Reservations.Add(new Reservation { IdReservation = 10, IdGuest = 1, IdRoom = 1, IdUser = 1, Capacity = 1, Price = 1m, Confirmed = false });
            ctx.SaveChanges();

            var sut = CreateSut(ctx);

            var r = await sut.DeleteGuest(1, 1, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task DeleteGuest_ShouldOk_RemoveGuestAndPerson()
        {
            using var ctx = CreateCtx(nameof(DeleteGuest_ShouldOk_RemoveGuestAndPerson));
            SeedBase(ctx);
            AddGuest(ctx, 1, 1, "A", "a@x.com", "111", "P1");

            var sut = CreateSut(ctx);

            var r = await sut.DeleteGuest(1, 1, CancellationToken.None);

            r.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            ctx.Guests.Any(g => g.IdPerson == 1).Should().BeFalse();
            ctx.Persons.Any(p => p.IdPerson == 1).Should().BeFalse();
        }
    }
}
