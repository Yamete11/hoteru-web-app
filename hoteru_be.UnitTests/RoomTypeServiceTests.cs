using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class RoomTypeServiceTests
{
    private MyDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_RoomType")
            .Options;

        var context = new MyDbContext(options);


        context.RoomTypes.AddRange(
            new RoomType { IdRoomType = 1, Title = "Regular" },
            new RoomType { IdRoomType = 2, Title = "Luxury" }
        );
        context.SaveChanges();

        return context;
    }

    [Fact]
    public async Task GetRoomTypes_ReturnsAllRoomTypes()
    {

        var context = GetInMemoryDbContext();
        var service = new RoomTypeService(context);


        IEnumerable<TypeDTO> result = await service.GetRoomTypes();

        Assert.NotNull(result);
        Assert.Collection(result,
            item => Assert.Equal("Regular", item.Title),
            item => Assert.Equal("Luxury", item.Title));
    }
}
;