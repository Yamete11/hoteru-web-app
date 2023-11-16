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

        public async Task<IEnumerable<RoomDTO>> GetRooms(int page, int limit)
        {
            return await _context.Rooms
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
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

        public async Task<MethodResultDTO> PostRoom(RoomDTO roomDTO)
        {
            Room room = new Room
            {
                Number = roomDTO.Number,
                Capacity = roomDTO.Capacity,
                Price = roomDTO.Price,
                IdRoomType = int.Parse(roomDTO.Type),
                IdRoomStatus= int.Parse(roomDTO.Status)
            };
            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }
    }
}
