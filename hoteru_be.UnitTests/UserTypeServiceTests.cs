using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace hoteru_be.UnitTests
{
    public class UserTypeServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_UserType")
                .Options;

            var context = new MyDbContext(options);

            context.UserTypes.AddRange(
                new UserType { IdUserType = 1, Title = "Admin" },
                new UserType { IdUserType = 2, Title = "Superadmin" },
                new UserType { IdUserType = 3, Title = "Employee" }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetUserTypes_ReturnsAllUserTypes()
        {
            var context = GetInMemoryDbContext();
            var service = new UserTypeService(context);

            IEnumerable<TypeDTO> result = await service.GetUserTypes();

            Assert.NotNull(result);
            Assert.Collection(result,
                item => Assert.Equal("Admin", item.Title),
                item => Assert.Equal("Superadmin", item.Title),
                item => Assert.Equal("Employee", item.Title));
        }
    }
}
