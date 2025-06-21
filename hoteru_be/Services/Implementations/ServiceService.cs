using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
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

        public ServiceService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<MethodResultDTO> DeleteService(int idService)
        {
            Service service = _context.Services.SingleOrDefault(x => x.IdService == idService);

            _context.Services.Remove(service);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery)
        {
            IQueryable<Service> query = _context.Services;

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
            var service = await _context.Services.FirstOrDefaultAsync(x => x.IdService == idService);

            return new ServiceDTO
            {
                IdService = service.IdService,
                Title= service.Title,
                Sum= service.Sum,
                Description= service.Description
            };
        }

        public async Task<MethodResultDTO> PostService(ServiceDTO serviceDTO)
        {
            Service service = new Service
            {
                Title = serviceDTO.Title,
                Sum = serviceDTO.Sum.Value,
                Description= serviceDTO.Description
            };
            _context.Services.Add(service);

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }

        public async Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO)
        {
            var service = await _context.Services
                             .FirstOrDefaultAsync(r => r.IdService == serviceDTO.IdService);

            if (service == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Service not found"
                };
            }

            service.Title = serviceDTO.Title;
            service.Sum = serviceDTO.Sum.Value;
            service.Description = serviceDTO.Description;

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };

        }
    }
}
