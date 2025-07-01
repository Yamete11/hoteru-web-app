using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class ServiceService : IServiceService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceService(MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<MethodResultDTO> DeleteService(int idService)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();

            var service = await _context.Services
                .Include(s => s.User)
                .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(x => x.IdService == idService && x.User.Person.IdHotel == hotelId);

            if (service == null)
                return NotFound("Service not found");

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return Success("Deleted");
        }

        public async Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return new PaginatedResultDTO<ServiceDTO>
            {
                List = new List<ServiceDTO>(),
                TotalCount = 0,
                Page = page,
                Limit = limit
            };


            IQueryable<Service> query = _context.Services
                .Include(s => s.User)
                .ThenInclude(u => u.Person)
                .Where(s => s.User.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();

                switch (searchField.ToLower())
                {
                    case "title":
                        query = query.Where(s => s.Title.ToLower().StartsWith(searchQuery));
                        break;
                    case "description":
                        query = query.Where(s => s.Description.ToLower().StartsWith(searchQuery));
                        break;
                    case "sum":
                        if (decimal.TryParse(searchQuery, out var sumValue))
                        {
                            query = query.Where(s => s.Sum.ToString().StartsWith(searchQuery));
                        }
                        else
                        {
                            query = query.Where(s => false);
                        }
                        break;
                }
            }

            var totalServices = await query.CountAsync();

            var services = await query
                .OrderBy(r => r.IdService)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new ServiceDTO
                {
                    IdService = x.IdService,
                    Title = x.Title,
                    Sum = x.Sum,
                    Description = x.Description
                })
                .ToListAsync();

            return new PaginatedResultDTO<ServiceDTO>
            {
                List = services,
                TotalCount = totalServices,
                Page = page,
                Limit = limit
            };
        }

        public async Task<ServiceDTO> GetSpecificService(int idService)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return null;

            var service = await _context.Services
                .Include(s => s.User)
                .ThenInclude(u => u.Person)
                .Where(s => s.IdService == idService && s.User.Person.IdHotel == hotelId)
                .FirstOrDefaultAsync();

            if (service == null) return null;

            return new ServiceDTO
            {
                IdService = service.IdService,
                Title = service.Title,
                Sum = service.Sum,
                Description = service.Description
            };
        }

        public async Task<MethodResultDTO> PostService(ServiceDTO serviceDTO)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();


            var service = new Service
            {
                Title = serviceDTO.Title,
                Sum = serviceDTO.Sum ?? 0,
                Description = serviceDTO.Description,
                User = await _context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Person.IdHotel == hotelId)
            };


            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Success("Created");
        }

        public async Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();

            var service = await _context.Services
                .Include(s => s.User)
                .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(s => s.IdService == serviceDTO.IdService && s.User.Person.IdHotel == hotelId);

            if (service == null)
                return NotFound("Service not found");

            service.Title = serviceDTO.Title;
            service.Sum = serviceDTO.Sum ?? 0;
            service.Description = serviceDTO.Description;

            await _context.SaveChangesAsync();
            return Success("Updated");
        }


        private MethodResultDTO ForbiddenResult() => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.Forbidden,
            Message = "Hotel ID missing or invalid"
        };

        private MethodResultDTO NotFound(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.NotFound,
            Message = message
        };

        private MethodResultDTO Error(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            Message = message
        };

        private MethodResultDTO Success(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.OK,
            Message = message
        };
    }
}
