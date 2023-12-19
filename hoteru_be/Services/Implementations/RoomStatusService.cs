using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class RoomStatusService : IRoomStatusService
    {
        private readonly MyDbContext _context;

        public RoomStatusService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StatusDTO>> GetRoomStatuses()
        {
            return await _context.RoomStatuses.Select(x => new StatusDTO
            {
                IdStatus = x.IdRoomStatus,
                Title = x.Title,
            }).ToListAsync();
        }
    }
}
