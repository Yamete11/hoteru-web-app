using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace hoteru_be.UnitTests
{
    public class ServiceServiceTests
    {
        private MyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            var context = new MyDbContext(options);

            context.Services.AddRange(
                new Service { IdService = 1, Title = "Cleaning", Sum = 50, Description = "Room cleaning" },
                new Service { IdService = 2, Title = "Laundry", Sum = 30, Description = "Laundry service" }
            );

            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task GetSpecificService_ReturnsCorrectService()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var result = await service.GetSpecificService(1);

            Assert.Equal("Cleaning", result.Title);
            Assert.Equal(50, result.Sum);
        }

        [Fact]
        public async Task PostService_CreatesService()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var newService = new ServiceDTO
            {
                Title = "Breakfast",
                Sum = 20,
                Description = "Morning meal"
            };

            var result = await service.PostService(newService);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.Equal(3, context.Services.Count());
        }

        [Fact]
        public async Task DeleteService_RemovesService()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var result = await service.DeleteService(1);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            Assert.Null(context.Services.FirstOrDefault(s => s.IdService == 1));
        }

        [Fact]
        public async Task UpdateService_UpdatesFields()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var updated = new ServiceDTO
            {
                IdService = 1,
                Title = "Deep Cleaning",
                Sum = 70,
                Description = "Full room deep cleaning"
            };

            var result = await service.UpdateService(updated);

            Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
            var s = context.Services.First(s => s.IdService == 1);
            Assert.Equal("Deep Cleaning", s.Title);
            Assert.Equal(70, s.Sum);
        }

        [Fact]
        public async Task GetServices_WithTitleSearch_ReturnsFiltered()
        {
            var context = GetInMemoryDbContext();
            var service = new ServiceService(context);

            var result = await service.GetServices(1, 10, "title", "clean");

            Assert.Single(result.List);
            Assert.Equal("Cleaning", result.List.ElementAt(0).Title);
        }
    }
}
