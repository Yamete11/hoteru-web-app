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
            Dictionary<string, string> validationErrors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(roomDTO.Number))
            {
                validationErrors.Add("Number", "Room number is required");
            }
            else if (roomDTO.Number.Length < 1)
            {
                validationErrors.Add("Number", "Room number is required");
            } 
            else if (roomDTO.Number.Length > 3)
            {
                validationErrors.Add("Number", "Room number must be at most 3 characters");
            }



            if (!roomDTO.Capacity.HasValue || roomDTO.Capacity.Value < 1 || roomDTO.Capacity.Value > 10)
            {
                validationErrors.Add("Capacity", "Room capacity should be between 1 and 10.");
            }



            if (!roomDTO.Price.HasValue)
            {
                validationErrors.Add("Price", "Room price is required");
            }


            if (string.IsNullOrWhiteSpace(roomDTO.Status))
            {
                validationErrors.Add("Status", "Room status is required");
            }
            if (string.IsNullOrWhiteSpace(roomDTO.Type))
            {
                validationErrors.Add("Type", "Room type is required");
            }

            if (validationErrors.Count > 0)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    ValidationErrors = validationErrors
                };
            }

            Room room = new Room
            {
                Number = roomDTO.Number,
                Capacity = (int)roomDTO.Capacity,
                Price = (int)roomDTO.Price,
                IdRoomType = int.Parse(roomDTO.Type),
                IdRoomStatus = int.Parse(roomDTO.Status)
            };
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
