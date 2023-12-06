using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class GuestStatusService : IGuestStatusService
    {
        private readonly MyDbContext _context;

        public GuestStatusService(MyDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<GuestStatusDTO>> GetGuestStatuses()
        {
            return await _context.GuestStatuses.Select(x => new GuestStatusDTO
            {
                IdGuestStatus = x.IdGuestStatus,
                Title = x.Title,
            }).ToListAsync();
        }
    }
}
