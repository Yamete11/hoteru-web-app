using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class RoomService : IRoomService
    {

        private readonly MyDbContext _context;

        public RoomService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomDTO>> GetRooms()
        {
            return await _context.Rooms.Select(x => new RoomDTO
            {
                IdRoom = x.IdRoom,
                Number = x.Number,
                Capacity = x.Capacity,
                Price = x.Price,
                Status = x.RoomStatus.Title,
                Type = x.RoomType.Title
            }).ToListAsync();
        }
    }
}
