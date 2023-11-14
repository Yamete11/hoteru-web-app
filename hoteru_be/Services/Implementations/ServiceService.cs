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

        public async Task<MethodResultDTO> DeleteService(int IdService)
        {
            Service service = _context.Services.SingleOrDefault(x => x.IdService == IdService);

            _context.Services.Remove(service);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<IEnumerable<ServiceDTO>> GetServices()
        {
            return await _context.Services.Select(x => new ServiceDTO
            {
                IdService = x.IdService,
                Title = x.Title,
                Sum = x.Sum,
                Description = x.Description
            }).ToListAsync();
        }
    }
}
