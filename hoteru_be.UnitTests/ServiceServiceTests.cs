using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;


public class ServiceServiceTests
{
    private MyDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                 .Options;

        var context = new MyDbContext(options);

        var hotelId = 1;
        var user = new User
        {
            IdPerson = 1,
            Person = new Person { IdHotel = hotelId }
        };
        context.Users.Add(user);

        var services = new List<Service>
        {
            new Service { IdService = 1, Title = "Cleaning", Description = "Room cleaning", Sum = 100, User = user, IdUser = user.IdPerson },
            new Service { IdService = 2, Title = "Breakfast", Description = "Breakfast service", Sum = 50, User = user, IdUser = user.IdPerson },
        };

        context.Services.AddRange(services);
        context.SaveChanges();

        return context;
    }

    private Mock<IHttpContextAccessor> GetHttpContextAccessorMock(int? hotelId = 1, int? userId = 1)
    {
        var claims = new List<Claim>();
        if (hotelId.HasValue)
            claims.Add(new Claim("hotelId", hotelId.Value.ToString()));
        if (userId.HasValue)
            claims.Add(new Claim("id", userId.Value.ToString()));

        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContextMock.Object);

        return httpContextAccessorMock;
    }

    [Fact]
    public async Task GetServices_ReturnsServices_ForValidHotelId()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);


        var result = await serviceService.GetServices(1, 10, null, null);


        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.List.Count());
        Assert.Contains(result.List, s => s.Title == "Cleaning");
    }

    [Fact]
    public async Task GetServices_ReturnsEmpty_ForMissingHotelId()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: null);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var result = await serviceService.GetServices(1, 10, null, null);

        Assert.NotNull(result);
        Assert.Empty(result.List);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task DeleteService_DeletesExistingService()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var serviceIdToDelete = 1;

        var beforeCount = context.Services.Count();

        var result = await serviceService.DeleteService(serviceIdToDelete);

        var afterCount = context.Services.Count();

        Assert.Equal(beforeCount - 1, afterCount);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.Equal("Deleted", result.Message);
    }

    [Fact]
    public async Task DeleteService_ReturnsNotFound_IfServiceNotExist()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var result = await serviceService.DeleteService(999);

        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.Equal("Service not found", result.Message);
    }

    [Fact]
    public async Task PostService_CreatesService_ForValidUser()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1, userId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var newServiceDto = new ServiceDTO
        {
            Title = "Laundry",
            Sum = 30,
            Description = "Laundry service"
        };

        var beforeCount = context.Services.Count();

        var result = await serviceService.PostService(newServiceDto);

        var afterCount = context.Services.Count();

        Assert.Equal(beforeCount + 1, afterCount);
        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.Equal("Created", result.Message);
    }

    [Fact]
    public async Task PostService_ReturnsForbidden_IfHotelIdMissing()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: null);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var result = await serviceService.PostService(new ServiceDTO { Title = "Test" });

        Assert.Equal(HttpStatusCode.Forbidden, result.HttpStatusCode);
        Assert.Equal("Hotel ID missing or invalid", result.Message);
    }

    [Fact]
    public async Task UpdateService_UpdatesExistingService()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var updateDto = new ServiceDTO
        {
            IdService = 1,
            Title = "Updated Cleaning",
            Sum = 150,
            Description = "Updated description"
        };

        var result = await serviceService.UpdateService(updateDto);

        var updatedService = await context.Services.FindAsync(1);

        Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
        Assert.Equal("Updated", result.Message);
        Assert.Equal("Updated Cleaning", updatedService.Title);
        Assert.Equal(150, updatedService.Sum);
        Assert.Equal("Updated description", updatedService.Description);
    }

    [Fact]
    public async Task UpdateService_ReturnsNotFound_ForInvalidService()
    {
        var context = GetDbContext();
        var httpContextAccessor = GetHttpContextAccessorMock(hotelId: 1);

        var serviceService = new ServiceService(context, httpContextAccessor.Object);

        var updateDto = new ServiceDTO
        {
            IdService = 999,
            Title = "No service",
            Sum = 0,
            Description = "N/A"
        };

        var result = await serviceService.UpdateService(updateDto);

        Assert.Equal(HttpStatusCode.NotFound, result.HttpStatusCode);
        Assert.Equal("Service not found", result.Message);
    }
}
