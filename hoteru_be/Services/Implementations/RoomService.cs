using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public async Task<SpecificRoomDTO> GetSpecificRoom(int IdRoom)
        {
            var room = await _context.Rooms
            .Include(r => r.RoomStatus) 
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(x => x.IdRoom == IdRoom);

            return new SpecificRoomDTO
            {
                IdRoom = room.IdRoom,
                Number = room.Number,
                Capacity = room.Capacity,
                Price = room.Price,
                Status = room.IdRoomStatus,
                Type = room.IdRoomType 
            };
        }

        public async Task<MethodResultDTO> PostRoom(RoomDTO roomDTO)
        {
            var existingRoom = await _context.Rooms
                            .AnyAsync(r => r.Number == roomDTO.Number);

            if (existingRoom)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "A room with this number already exists"
                };
            }


            Room room = new Room
            {
                Number = roomDTO.Number,
                Capacity = roomDTO.Capacity.Value,
                Price = roomDTO.Price.Value,
                IdRoomType = int.Parse(roomDTO.Type),
                IdRoomStatus = int.Parse(roomDTO.Status)
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(room, null, null);

            if (!Validator.TryValidateObject(room, validationContext, validationResults, true))
            {
                var validationErrors = validationResults.ToDictionary(
                    vr => vr.MemberNames.First(),
                    vr => vr.ErrorMessage
                );

                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    ValidationErrors = validationErrors
                };
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }


        public async Task<MethodResultDTO> UpdateRoom(SpecificRoomDTO roomDTO)
        {
            var room = await _context.Rooms
                             .Include(r => r.RoomStatus)
                             .Include(r => r.RoomType)
                             .FirstOrDefaultAsync(r => r.IdRoom == roomDTO.IdRoom);

            if (room == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Room not found"
                };
            }

            var existingRoom = await _context.Rooms
                .AnyAsync(r => r.Number == roomDTO.Number && r.IdRoom != roomDTO.IdRoom);

            if (existingRoom)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Another room with this number already exists"
                };
            }

            room.Number = roomDTO.Number;
            room.Capacity = roomDTO.Capacity;
            room.Price = roomDTO.Price;
            room.IdRoomStatus = roomDTO.Status;
            room.IdRoomType = roomDTO.Type;

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };

        }
    }
}
