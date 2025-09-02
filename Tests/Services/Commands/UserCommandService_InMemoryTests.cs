using FluentAssertions;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace hoteru_be.Tests.Services.Commands
{
    public class UserCommandService_InMemoryTests
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
            var super = new UserType { IdUserType = 1, Title = "Superadmin" };
            var admin = new UserType { IdUserType = 2, Title = "Admin" };
            var employee = new UserType { IdUserType = 3, Title = "Employee" };
            ctx.UserTypes.AddRange(super, admin, employee);

            var h1 = new Hotel { IdHotel = 1, Title = "Hotel One", Address = new Address { IdAddress = 1, City = "W", Country = "PL", Street = "S1", Postcode = "00-001" } };
            var h2 = new Hotel { IdHotel = 2, Title = "Hotel Two", Address = new Address { IdAddress = 2, City = "K", Country = "PL", Street = "S2", Postcode = "30-001" } };
            ctx.Hotels.AddRange(h1, h2);

            var pSuper = new Person { IdPerson = 1, IdHotel = 1, Name = "Root", Surname = "S", Email = "root@x.com", Hotel = h1 };
            var pAdmin = new Person { IdPerson = 2, IdHotel = 1, Name = "A", Surname = "A", Email = "admin@x.com", Hotel = h1 };
            var pEmp = new Person { IdPerson = 3, IdHotel = 1, Name = "E", Surname = "E", Email = "emp@x.com", Hotel = h1 };
            var pOther = new Person { IdPerson = 4, IdHotel = 2, Name = "O", Surname = "O", Email = "other@x.com", Hotel = h2 };
            ctx.Persons.AddRange(pSuper, pAdmin, pEmp, pOther);

            var uSuper = new User { IdPerson = 1, Person = pSuper, IdUserType = 1, UserType = super, LoginName = "root", Password = "x" };
            var uAdmin = new User { IdPerson = 2, Person = pAdmin, IdUserType = 2, UserType = admin, LoginName = "admin", Password = "x" };
            var uEmp = new User { IdPerson = 3, Person = pEmp, IdUserType = 3, UserType = employee, LoginName = "emp", Password = "x" };
            var uOther = new User { IdPerson = 4, Person = pOther, IdUserType = 3, UserType = employee, LoginName = "other", Password = "x" };
            ctx.Users.AddRange(uSuper, uAdmin, uEmp, uOther);

            ctx.SaveChanges();
        }

        private static UserCommandService CreateSut(MyDbContext ctx)
        {
            var logger = new Mock<ILogger<UserCommandService>>();

            var hasher = new Mock<IPasswordHasher<User>>();
            hasher
                .Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns((User _, string pwd) => "HASH:" + pwd);

            return new UserCommandService(ctx, hasher.Object, logger.Object);
        }


        [Fact]
        public async Task PostUser_ShouldCreate_WhenUniqueAndTypeExists()
        {
            using var ctx = CreateCtx(nameof(PostUser_ShouldCreate_WhenUniqueAndTypeExists));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new NewUserDTO
            {
                Name = "N",
                Surname = "S",
                Email = "new@x.com",
                LoginName = "newuser",
                Password = "p@ss",
                IdUserType = 3
            };

            var res = await sut.PostUser(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Created);
            ctx.Users.Any(u => u.LoginName == "newuser").Should().BeTrue();
            var created = ctx.Users.Include(u => u.Person).Single(u => u.LoginName == "newuser");
            created.Person.Email.Should().Be("new@x.com");
            created.IdUserType.Should().Be(3);
            created.Password.Should().StartWith("HASH:");
        }

        [Fact]
        public async Task PostUser_ShouldFail_WhenDuplicateLogin()
        {
            using var ctx = CreateCtx(nameof(PostUser_ShouldFail_WhenDuplicateLogin));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new NewUserDTO
            {
                Name = "N",
                Surname = "S",
                Email = "unique@x.com",
                LoginName = "admin",
                Password = "p",
                IdUserType = 3
            };

            var res = await sut.PostUser(1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Errors.Should().ContainKey("LoginName");
        }

        [Fact]
        public async Task PostUser_ShouldFail_WhenDuplicateEmailInSameHotel()
        {
            using var ctx = CreateCtx(nameof(PostUser_ShouldFail_WhenDuplicateEmailInSameHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostUser(1, new NewUserDTO
            {
                Name = "N",
                Surname = "S",
                Email = "admin@x.com",
                LoginName = "newu",
                Password = "p",
                IdUserType = 3
            }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Errors.Should().ContainKey("Email");
        }

        [Fact]
        public async Task PostUser_ShouldSucceed_WhenDuplicateEmailButOtherHotel()
        {
            using var ctx = CreateCtx(nameof(PostUser_ShouldSucceed_WhenDuplicateEmailButOtherHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostUser(2, new NewUserDTO
            {
                Name = "N",
                Surname = "S",
                Email = "admin@x.com",
                LoginName = "ok2",
                Password = "p",
                IdUserType = 3
            }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task PostUser_ShouldFail_WhenUserTypeMissing()
        {
            using var ctx = CreateCtx(nameof(PostUser_ShouldFail_WhenUserTypeMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.PostUser(1, new NewUserDTO
            {
                Name = "N",
                Surname = "S",
                Email = "x@x.com",
                LoginName = "x1",
                Password = "p",
                IdUserType = 999
            }, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("User type not found");
        }

        [Fact]
        public async Task UpdateUser_ShouldAllowEmployeeSelfEdit_WithoutRoleChange()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldAllowEmployeeSelfEdit_WithoutRoleChange));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E2",
                Surname = "E2",
                Email = "emp2@x.com",
                LoginName = "emp2",
                IdUserType = 2
            };

            var res = await sut.UpdateUser(1, currentRole: "Employee", currentPersonId: 3, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            var u = ctx.Users.Include(x => x.Person).Single(x => x.IdPerson == 3);
            u.Person.Name.Should().Be("E2");
            u.LoginName.Should().Be("emp2");
            u.IdUserType.Should().Be(3);
        }

        [Fact]
        public async Task UpdateUser_ShouldForbidEmployeeEditingOthers()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldForbidEmployeeEditingOthers));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 2,
                Name = "A2",
                Surname = "A2",
                Email = "admin2@x.com",
                LoginName = "admin2",
                IdUserType = 3
            };

            var res = await sut.UpdateUser(1, currentRole: "Employee", currentPersonId: 3, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateUser_ShouldAllowAdminAssignEmployeeOrAdmin()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldAllowAdminAssignEmployeeOrAdmin));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E3",
                Surname = "E3",
                Email = "emp3@x.com",
                LoginName = "emp3",
                IdUserType = 2
            };

            var res = await sut.UpdateUser(1, currentRole: "Admin", currentPersonId: 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.OK);
            ctx.Users.Single(x => x.IdPerson == 3).IdUserType.Should().Be(2);
        }

        [Fact]
        public async Task UpdateUser_ShouldForbidAdminAssigningSuperadmin()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldForbidAdminAssigningSuperadmin));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E",
                Surname = "E",
                Email = "emp@x.com",
                LoginName = "emp",
                IdUserType = 1
            };

            var res = await sut.UpdateUser(1, currentRole: "Admin", currentPersonId: 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateUser_ShouldForbidSuperadminGrantingSuperadminToAnother()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldForbidSuperadminGrantingSuperadminToAnother));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 2,
                Name = "A",
                Surname = "A",
                Email = "admin@x.com",
                LoginName = "admin",
                IdUserType = 1
            };

            var res = await sut.UpdateUser(1, currentRole: "Superadmin", currentPersonId: 1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateUser_ShouldForbidSuperadminChangingOwnRole()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldForbidSuperadminChangingOwnRole));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 1,
                Name = "Root",
                Surname = "S",
                Email = "root@x.com",
                LoginName = "root",
                IdUserType = 2
            };

            var res = await sut.UpdateUser(1, currentRole: "Superadmin", currentPersonId: 1, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateUser_ShouldFail_WhenLoginExists()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldFail_WhenLoginExists));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E",
                Surname = "E",
                Email = "emp@x.com",
                LoginName = "admin",
                IdUserType = 3
            };

            var res = await sut.UpdateUser(1, currentRole: "Admin", currentPersonId: 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Errors.Should().ContainKey("LoginName");
        }

        [Fact]
        public async Task UpdateUser_ShouldFail_WhenEmailExistsInHotel()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldFail_WhenEmailExistsInHotel));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E",
                Surname = "E",
                Email = "admin@x.com",
                LoginName = "emp",
                IdUserType = 3
            };

            var res = await sut.UpdateUser(1, currentRole: "Admin", currentPersonId: 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.BadRequest);
            res.Errors.Should().ContainKey("Email");
        }

        [Fact]
        public async Task UpdateUser_ShouldFail_WhenTargetNotFound()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldFail_WhenTargetNotFound));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 999,
                Name = "X",
                Surname = "X",
                Email = "x@x.com",
                LoginName = "x",
                IdUserType = 3
            };

            var res = await sut.UpdateUser(1, "Admin", 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateUser_ShouldFail_WhenRequestedRoleMissing()
        {
            using var ctx = CreateCtx(nameof(UpdateUser_ShouldFail_WhenRequestedRoleMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var dto = new UpdateUserDTO
            {
                IdPerson = 3,
                Name = "E",
                Surname = "E",
                Email = "emp@x.com",
                LoginName = "emp",
                IdUserType = 999
            };

            var res = await sut.UpdateUser(1, "Admin", 2, dto, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
            res.Message.Should().Be("User type not found");
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenMissing()
        {
            using var ctx = CreateCtx(nameof(DeleteUser_ShouldReturnNotFound_WhenMissing));
            SeedBasics(ctx);
            var sut = CreateSut(ctx);

            var res = await sut.DeleteUser(1, idPerson: 999, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnError_WhenNoSuperadminUser()
        {
            using var ctx = CreateCtx(nameof(DeleteUser_ShouldReturnError_WhenNoSuperadminUser));
            SeedBasics(ctx);
            var super = ctx.Users.Single(u => u.IdUserType == 1);
            ctx.Users.Remove(super);
            ctx.Persons.Remove(ctx.Persons.Single(p => p.IdPerson == super.IdPerson));
            ctx.SaveChanges();

            var sut = CreateSut(ctx);

            var res = await sut.DeleteUser(1, idPerson: 2, CancellationToken.None);

            res.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
            res.Message.Should().Be("Super admin user not found");
        }



    }
}
