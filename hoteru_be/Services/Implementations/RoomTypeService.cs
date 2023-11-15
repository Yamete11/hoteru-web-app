using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly MyDbContext _context;

        public RoomTypeService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomTypeDTO>> GetRoomTypes()
        {
            return await _context.RoomTypes.Select(x => new RoomTypeDTO
            {
                IdRoomType = x.IdRoomType,
                Title = x.Title,
            }).ToListAsync();
        }
    }
}
