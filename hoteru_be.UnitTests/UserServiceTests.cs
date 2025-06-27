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
    public class UserServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new MyDbContext(options);

            var superAdminUserType = new UserType { Title = "SuperAdmin" };
            var normalUserType = new UserType { Title = "User" };
            context.UserTypes.AddRange(superAdminUserType, normalUserType);

            var hotel = new Hotel { Title = "Test Hotel" };
            context.Hotels.Add(hotel);

            context.SaveChanges();

            var superAdminPerson = new Person { Name = "Super", Surname = "Admin", Email = "super@hotel.com", Hotel = hotel };
            context.Persons.Add(superAdminPerson);
            context.SaveChanges();

            var superAdminUser = new User
            {
                LoginName = "superadmin",
                Password = "pass",
                IdUserType = superAdminUserType.IdUserType,
                Person = superAdminPerson,
                Reservations = new List<Reservation>()
            };
            context.Users.Add(superAdminUser);

            context.SaveChanges();

            return context;
        }


        [Fact]
        public async Task DeleteUser_UserNotFound_ReturnsNotFound()
        {
            var context = GetInMemoryDbContext();
            var service = new UserService(context);

            var result = await service.DeleteUser(999);

            Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
            Assert.Equal("User or Person not found", result.Message);
        }

        [Fact]
        public async Task DeleteUser_SuperAdminNotFound_ReturnsInternalServerError()
        {
            var context = GetInMemoryDbContext();

            var superAdmin = await context.Users.Include(u => u.UserType).FirstAsync(u => u.UserType.Title == "SuperAdmin");
            context.Users.Remove(superAdmin);
            await context.SaveChangesAsync();

            var hotel = await context.Hotels.FirstAsync();

            var person = new Person { Name = "Test", Surname = "User", Email = "test@hotel.com", Hotel = hotel };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var user = new User
            {
                LoginName = "testuser",
                Password = "123",
                IdUserType = (await context.UserTypes.FirstAsync(ut => ut.Title == "User")).IdUserType,
                Person = person,
                Reservations = new List<Reservation>()
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context);

            var result = await service.DeleteUser(user.IdPerson);

            Assert.Equal(HttpStatusCode.InternalServerError, result.HttpStatusCode);
            Assert.Equal("Super admin user not found", result.Message);
        }

        [Fact]
        public async Task DeleteUser_ValidUser_DeletesSuccessfully()
        {
            var context = GetInMemoryDbContext();

            var hotel = await context.Hotels.FirstAsync();

            var person = new Person { Name = "John", Surname = "Doe", Email = "john@hotel.com", Hotel = hotel };
            context.Persons.Add(person);
            await context.SaveChangesAsync();

            var userType = await context.UserTypes.FirstAsync(ut => ut.Title == "User");

            var user = new User
            {
                LoginName = "johndoe",
                Password = "pwd",
                IdUserType = userType.IdUserType,
                Person = person,
                Reservations = new List<Reservation>
                {
                    new Reservation()
                }
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var reservation = await context.Reservations.FirstAsync();

            var service = new UserService(context);

            var result = await service.DeleteUser(user.IdPerson);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.Equal("Deleted", result.Message);

            Assert.Null(await context.Users.FindAsync(user.IdPerson));
            Assert.Null(await context.Persons.FindAsync(person.IdPerson));

            var updatedReservation = await context.Reservations.FirstOrDefaultAsync(r => r.IdReservation == reservation.IdReservation);
            Assert.NotNull(updatedReservation);
            var superAdminUser = await context.Users.Include(u => u.UserType).FirstAsync(u => u.UserType.Title == "SuperAdmin");
            Assert.Equal(superAdminUser.IdPerson, updatedReservation.IdUser);
        }

        [Fact]
        public async Task PostUser_DuplicateLogin_ReturnsBadRequest()
        {
            var context = GetInMemoryDbContext();

            var service = new UserService(context);

            var newUser = new NewUserDTO
            {
                LoginName = "superadmin",
                Password = "123",
                IdUserType = (await context.UserTypes.FirstAsync(ut => ut.Title == "User")).IdUserType,
                Name = "Test",
                Surname = "User",
                Email = "test@hotel.com"
            };

            var result = await service.PostUser(newUser);

            Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
            Assert.Contains("LoginName", result.Errors.Keys);
        }

        [Fact]
        public async Task PostUser_ValidUser_CreatesSuccessfully()
        {
            var context = GetInMemoryDbContext();

            var service = new UserService(context);

            var newUser = new NewUserDTO
            {
                LoginName = "newuser",
                Password = "123",
                IdUserType = (await context.UserTypes.FirstAsync(ut => ut.Title == "User")).IdUserType,
                Name = "Test",
                Surname = "User",
                Email = "test@hotel.com"
            };

            var result = await service.PostUser(newUser);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.Equal("Created", result.Message);

            var userInDb = await context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.LoginName == "newuser");
            Assert.NotNull(userInDb);
            Assert.Equal("Test", userInDb.Person.Name);
        }
    }
}
