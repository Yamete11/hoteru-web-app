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

            if(room.IdRoomStatus == 2)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotAcceptable,
                    Message = "You cannot delete an occupied room"
                };
            }

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<List<RoomDTO>> GetFreeRooms(int idRoom)
        {
            if(idRoom != 0)
            {
                return await _context.Rooms.
                Where(x => x.IdRoomStatus == 3 || x.IdRoom == idRoom)
                .Select(x => new RoomDTO
                {
                    IdRoom = x.IdRoom,
                    Number = x.Number,
                    Capacity = x.Capacity,
                    Price = x.Price,
                    Status = x.RoomStatus.Title,
                    Type = x.RoomType.Title
                }).ToListAsync();
            }

            return await _context.Rooms.
                Where(x => x.IdRoomStatus == 3)
                .Select(x => new RoomDTO
                {
                    IdRoom = x.IdRoom,
                    Number = x.Number,
                    Capacity = x.Capacity,
                    Price = x.Price,
                    Status = x.RoomStatus.Title,
                    Type = x.RoomType.Title
                }).ToListAsync();
        }

        public async Task<PaginatedResultDTO<RoomDTO>> GetRooms(int page, int limit, string searchQuery = "", string searchField = "number")
        {
            var query = _context.Rooms
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();

                switch (searchField.ToLower())
                {
                    case "number":
                        query = query.Where(r => r.Number.ToLower().StartsWith(searchQuery));
                        break;
                    case "capacity":
                        if (int.TryParse(searchQuery, out int cap))
                            query = query.Where(r => r.Capacity == cap);
                        break;
                    case "type":
                        query = query.Where(r => r.RoomType.Title.ToLower().StartsWith(searchQuery));
                        break;
                    case "status":
                        query = query.Where(r => r.RoomStatus.Title.ToLower().StartsWith(searchQuery));
                        break;
                }
            }

            var totalRooms = await query.CountAsync();

            var rooms = await query
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
                }).ToListAsync();

            return new PaginatedResultDTO<RoomDTO>
            {
                List = rooms,
                TotalCount = totalRooms,
                Page = page,
                Limit = limit
            };
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

            var checkRoom = await _context.Rooms.FirstOrDefaultAsync(r => r.Number == roomDTO.Number);

            if (checkRoom != null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Room number already exists.",
                    Errors = new Dictionary<string, List<string>>
                    {
                        { "Number", new List<string> { "Room number already exists." } }
                    }
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


            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }


        public async Task<MethodResultDTO> UpdateRoom(RoomDTO roomDTO)
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
                    Message = "Another room with this number already exists",
                    Errors = new Dictionary<string, List<string>>
                    {
                        { "Number", new List<string> { "The Room number already exists." } }
                    }
                };
            }

            room.Number = roomDTO.Number;
            room.Capacity = roomDTO.Capacity.Value;
            room.Price = roomDTO.Price.Value;
            room.IdRoomStatus = int.Parse(roomDTO.Status);
            room.IdRoomType = int.Parse(roomDTO.Type);

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };

        }
    }
}
