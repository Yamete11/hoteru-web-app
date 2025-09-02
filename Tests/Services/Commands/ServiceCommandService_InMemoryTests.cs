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
    public class ServiceCommandService_InMemoryTests
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

            ctx.Services.AddRange(
                new Service { IdService = 1, Title = "Spa", Sum = 30m, Description = "All day", User = u1 },
                new Service { IdService = 2, Title = "Breakfast", Sum = 15m, Description = "Buffet", User = u1 },
                new Service { IdService = 3, Title = "Transfer", Sum = 50m, Description = null, User = u3 }
            );

            ctx.SaveChanges();
        }

        private static ServiceCommandService CreateSut(MyDbContext ctx)
        {
            var logger = new Mock<ILogger<ServiceCommandService>>();
            return new ServiceCommandService(ctx, logger.Object);
        }

        [Fact]
        public async Task PostService_ShouldCreate_WhenUniqueTitleInHotel()
        {
            using var ctx = CreateCtx(nameof(PostService_ShouldCreate_WhenUniqueTitleInHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new ServiceDTO { Title = "Parking", Sum = 25m, Description = "Underground" };

            var res = await sut.PostService(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Services.Any(s => s.Title == "Parking" && s.User.Person.IdHotel == 1).Should().BeTrue();
        }

        [Fact]
        public async Task PostService_ShouldFail_WhenTitleExistsInSameHotel_IgnoringCase()
        {
            using var ctx = CreateCtx(nameof(PostService_ShouldFail_WhenTitleExistsInSameHotel_IgnoringCase));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostService(1, new ServiceDTO { Title = "spa", Sum = 30m }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Message.Should().Be("Service with this title already exists.");
            res.Errors.Should().ContainKey("Title");
        }

        [Fact]
        public async Task PostService_ShouldError_WhenNoUserForHotel()
        {
            using var ctx = CreateCtx(nameof(PostService_ShouldError_WhenNoUserForHotel));
            SeedBasics(ctx);
            ctx.Users.RemoveRange(ctx.Users.Where(u => u.Person.IdHotel == 2));
            ctx.SaveChanges();
            var sut = CreateSut(ctx);

            var res = await sut.PostService(2, new ServiceDTO { Title = "Laundry", Sum = 10m }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            res.Message.Should().Be("No user found for this hotel.");
        }

        [Fact]
        public async Task UpdateService_ShouldUpdate_WhenFoundInHotel()
        {
            using var ctx = CreateCtx(nameof(UpdateService_ShouldUpdate_WhenFoundInHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new ServiceDTO { IdService = 2, Title = "Breakfast+", Sum = 18m, Description = "Buffet+" };

            var res = await sut.UpdateService(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var s = ctx.Services.Single(x => x.IdService == 2);
            s.Title.Should().Be("Breakfast+");
            s.Sum.Should().Be(18m);
            s.Description.Should().Be("Buffet+");
        }

        [Fact]
        public async Task UpdateService_ShouldNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(UpdateService_ShouldNotFound_WhenWrongHotelOrId));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.UpdateService(1, new ServiceDTO { IdService = 999, Title = "X" }, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.UpdateService(1, new ServiceDTO { IdService = 3, Title = "X" }, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task DeleteService_ShouldNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(DeleteService_ShouldNotFound_WhenWrongHotelOrId));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var r1 = await sut.DeleteService(1, idService: 999, CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.DeleteService(1, idService: 3, CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
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
        public async Task DeleteService_ShouldReturnConflict_WhenDbUpdateException()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(nameof(DeleteService_ShouldReturnConflict_WhenDbUpdateException))
                .Options;
            using var ctx = new ThrowingDbContext(options);
            SeedBasics(ctx);
            ctx.ThrowOnSave = false;

            var sut = CreateSut(ctx);

            var exists = ctx.Services.Single(s => s.IdService == 1);
            exists.Should().NotBeNull();

            ctx.ThrowOnSave = true;
            var res = await sut.DeleteService(1, idService: 1, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
            res.Message.Should().Be("Cannot delete service due to related data.");
        }
    }
}
