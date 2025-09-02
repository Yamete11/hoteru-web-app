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
    public class UserQueryService_InMemoryTests
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
            var ut1 = new UserType { IdUserType = 1, Title = "Manager" };
            var ut2 = new UserType { IdUserType = 2, Title = "Admin" };
            ctx.UserTypes.AddRange(ut1, ut2);

            var addr1 = new Address { IdAddress = 1, City = "Warsaw", Country = "Poland", Street = "Main 1", Postcode = "00-001" };
            var addr2 = new Address { IdAddress = 2, City = "Krakow", Country = "Poland", Street = "Rynek 2", Postcode = "30-002" };
            var h1 = new Hotel { IdHotel = 1, Title = "Hotel One", Address = addr1 };
            var h2 = new Hotel { IdHotel = 2, Title = "Hotel Two", Address = addr2 };
            ctx.Addresses.AddRange(addr1, addr2);
            ctx.Hotels.AddRange(h1, h2);

            var p1 = new Person { IdPerson = 1, IdHotel = 1, Name = "Alice", Surname = "A", Email = "alice@x.com", Hotel = h1 };
            var p2 = new Person { IdPerson = 2, IdHotel = 1, Name = "Bob", Surname = "B", Email = "bob@x.com", Hotel = h1 };
            var p3 = new Person { IdPerson = 3, IdHotel = 2, Name = "Charlie", Surname = "C", Email = "charlie@x.com", Hotel = h2 };
            ctx.Persons.AddRange(p1, p2, p3);

            var u1 = new User { IdPerson = 1, Person = p1, IdUserType = 1, UserType = ut1, LoginName = "alice", Password = "pwd" };
            var u2 = new User { IdPerson = 2, Person = p2, IdUserType = 2, UserType = ut2, LoginName = "bob", Password = "pwd" };
            var u3 = new User { IdPerson = 3, Person = p3, IdUserType = 1, UserType = ut1, LoginName = "charlie", Password = "pwd" };
            ctx.Users.AddRange(u1, u2, u3);

            ctx.SaveChanges();
        }

        [Fact]
        public async Task GetFullUser_ShouldReturnOk_WhenExists()
        {
            using var ctx = CreateCtx(nameof(GetFullUser_ShouldReturnOk_WhenExists));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var result = await sut.GetFullUser(hotelId: 1, idUser: 1, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().Be("Alice");
            result.Data.Surname.Should().Be("A");
            result.Data.Email.Should().Be("alice@x.com");
            result.Data.LoginName.Should().Be("alice");
            result.Data.IdUserType.Should().Be(1);
        }

        [Fact]
        public async Task GetFullUser_ShouldReturnNotFound_WhenWrongHotelOrId()
        {
            using var ctx = CreateCtx(nameof(GetFullUser_ShouldReturnNotFound_WhenWrongHotelOrId));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var r1 = await sut.GetFullUser(hotelId: 1, idUser: 999, ct: CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.GetFullUser(hotelId: 2, idUser: 1, ct: CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUser_ShouldReturnBadRequest_WhenLoginEmpty()
        {
            using var ctx = CreateCtx(nameof(GetUser_ShouldReturnBadRequest_WhenLoginEmpty));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var result = await sut.GetUser(hotelId: 1, userName: "   ", ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Message.Should().Be("Validation failed");
            result.Errors.Should().ContainKey("LoginName");
        }

        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenExists()
        {
            using var ctx = CreateCtx(nameof(GetUser_ShouldReturnOk_WhenExists));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var result = await sut.GetUser(hotelId: 1, userName: "bob", ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull();
            result.Data.LoginName.Should().Be("bob");
            result.Data.IdUser.Should().Be(2);
            result.Data.CompanyTitle.Should().Be("Hotel One");
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenWrongHotelOrLogin()
        {
            using var ctx = CreateCtx(nameof(GetUser_ShouldReturnNotFound_WhenWrongHotelOrLogin));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var r1 = await sut.GetUser(hotelId: 1, userName: "charlie", ct: CancellationToken.None);
            r1.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);

            var r2 = await sut.GetUser(hotelId: 1, userName: "nouser", ct: CancellationToken.None);
            r2.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnSortedList_ForHotel()
        {
            using var ctx = CreateCtx(nameof(GetUsers_ShouldReturnSortedList_ForHotel));
            Seed(ctx);
            var sut = new UserQueryService(ctx, new Mock<ILogger<UserQueryService>>().Object);

            var result = await sut.GetUsers(hotelId: 1, ct: CancellationToken.None);

            result.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be("Fetched");
            result.Data.Should().NotBeNull().And.HaveCount(2);
            result.Data.Select(x => x.LoginName).Should().Equal("alice", "bob");
            result.Data.Select(x => x.UserType).Should().Equal("Manager", "Admin");
        }
    }
}
