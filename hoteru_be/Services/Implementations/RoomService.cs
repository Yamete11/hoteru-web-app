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
    public class RoomService : IRoomService
    {

        private readonly MyDbContext _context;

        public RoomService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<MethodResultDTO> DeleteRoom(int IdRoom)
        {
            Room room = _context.Rooms.SingleOrDefault(x => x.IdRoom == IdRoom);

            if (room == null)
            {
                return new MethodResultDTO 
                { 
                    HttpStatusCode = HttpStatusCode.NotFound, 
                    Message = "Not Found" 
                };
            };

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<IEnumerable<RoomDTO>> GetRooms(int offset, int limit)
        {
            return await _context.Rooms
                .OrderBy(r => r.IdRoom)
                .Skip(offset)
                .Take(limit)
                .Select(x => new RoomDTO
                {
                    IdRoom = x.IdRoom,
                    Number = x.Number,
                    Capacity = x.Capacity,
                    Price = x.Price,
                    Status = x.RoomStatus.Title,
                    Type = x.RoomType.Title
                })
                .ToListAsync();
        }
    }
}
