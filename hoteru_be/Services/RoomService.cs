using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public class RoomService : IRoomService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RoomService> _logger;

        public RoomService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<RoomService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<MethodResultDTO> DeleteRoom(int idRoom, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("DeleteRoom unauthorized for room {RoomId}", idRoom);
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var occupiedStatusId = await _context.RoomStatuses
                .AsNoTracking()
                .Where(s => s.Title == "Occupied")
                .Select(s => s.IdRoomStatus)
                .FirstOrDefaultAsync(ct);

            var roomInfo = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.IdRoom == idRoom && r.User.Person.IdHotel == hotelId)
                .Select(r => new { r.IdRoom, r.IdRoomStatus })
                .FirstOrDefaultAsync(ct);

            if (roomInfo is null)
            {
                _logger.LogWarning("DeleteRoom not found: room {RoomId}, hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            if (roomInfo.IdRoomStatus == occupiedStatusId)
            {
                _logger.LogWarning("Attempt to delete occupied room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Conflict("You cannot delete an occupied room");
            }

            try
            {
                var stub = new Room { IdRoom = roomInfo.IdRoom };
                _context.Entry(stub).State = EntityState.Deleted;

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Room {RoomId} deleted in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DeleteRoom conflict for room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Conflict("Cannot delete room due to related data.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DeleteRoom canceled for room {RoomId}", idRoom);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Error("Unexpected error while deleting room.");
            }
        }


        public async Task<List<RoomDTO>> GetFreeRooms(int idRoom, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetFreeRooms request");
                return new List<RoomDTO>();
            }

            var readyStatusId = await _context.RoomStatuses
                .AsNoTracking()
                .Where(s => s.Title == "Ready")
                .Select(s => s.IdRoomStatus)
                .FirstOrDefaultAsync(ct);

            var query = _context.Rooms
                .AsNoTracking()
                .Where(r => r.User.Person.IdHotel == hotelId);

            if (idRoom != 0)
            {
                query = query.Where(x => x.IdRoom == idRoom || x.IdRoomStatus == readyStatusId);
            }
            else
            {
                query = query.Where(x => x.IdRoomStatus == readyStatusId);
            }

            var list = await query
                .OrderBy(x => x.IdRoom)
                .Select(x => new RoomDTO
                {
                    IdRoom = x.IdRoom,
                    Number = x.Number,
                    Capacity = x.Capacity,
                    Price = x.Price,
                    Status = x.RoomStatus.Title,
                    Type = x.RoomType.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched free rooms for hotel {HotelId}: count={Count}, includeRoomId={IncludeRoom}",
                hotelId, list.Count, idRoom);

            return list;
        }


        public async Task<PaginatedResultDTO<RoomDTO>> GetRooms(int page, int limit, string searchQuery = "", string searchField = "number", CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetRooms request");
                return new PaginatedResultDTO<RoomDTO>
                {
                    List = new List<RoomDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
            }

            var query = _context.Rooms
                .AsNoTracking()
                .Where(r => r.User.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var field = (searchField ?? "").Trim().ToLower();
                var term = $"{searchQuery.Trim().ToLower()}%";

                switch (field)
                {
                    case "number":
                        query = query.Where(r => EF.Functions.Like(r.Number.ToLower(), term));
                        break;

                    case "capacity":
                        if (int.TryParse(searchQuery, out var cap))
                        {
                            query = query.Where(r => r.Capacity == cap);
                        }
                        break;

                    case "type":
                        query = query.Where(r => EF.Functions.Like(r.RoomType.Title.ToLower(), term));
                        break;

                    case "status":
                        query = query.Where(r => EF.Functions.Like(r.RoomStatus.Title.ToLower(), term));
                        break;

                    default:
                        break;
                }
            }

            var total = await query.CountAsync(ct);

            var list = await query
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
                .ToListAsync(ct);

            _logger.LogInformation("Fetched rooms: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<RoomDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<MethodResultDTO<SpecificRoomDTO>> GetSpecificRoom(int idRoom, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("GetSpecificRoom unauthorized for room {RoomId}", idRoom);
                return MethodResultDTO<SpecificRoomDTO>.Unauthorized("HotelId claim missing");
            }

            var dto = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.IdRoom == idRoom && r.User.Person.IdHotel == hotelId)
                .Select(r => new SpecificRoomDTO
                {
                    IdRoom = r.IdRoom,
                    Number = r.Number,
                    Capacity = r.Capacity,
                    Price = r.Price,
                    Status = r.IdRoomStatus,
                    Type = r.IdRoomType
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                _logger.LogWarning("GetSpecificRoom not found: room {RoomId}, hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO<SpecificRoomDTO>.NotFound("Room not found");
            }

            _logger.LogInformation("Fetched room {RoomId} for hotel {HotelId}", idRoom, hotelId);
            return MethodResultDTO<SpecificRoomDTO>.Ok(dto, "Fetched");
        }


        public async Task<MethodResultDTO> PostRoom(RoomDTO roomDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var numberExists = await _context.Rooms
                .AnyAsync(r => r.Number == roomDTO.Number && r.User.Person.IdHotel == hotelId, ct);
            if (numberExists)
            {
                return MethodResultDTO.BadRequest(
                    "Room number already exists.",
                    new Dictionary<string, List<string>> {
                        { "Number", new List<string> { "Room number already exists." } }
                    });
            }

            if (!int.TryParse(roomDTO.Status, out var statusId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid status identifier",
                    new Dictionary<string, List<string>> { { "Status", new List<string> { "Status must be a numeric id." } } });
            }
            if (!int.TryParse(roomDTO.Type, out var typeId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid type identifier",
                    new Dictionary<string, List<string>> { { "Type", new List<string> { "Type must be a numeric id." } } });
            }

            var statusExists = await _context.RoomStatuses.AsNoTracking()
                .AnyAsync(s => s.IdRoomStatus == statusId, ct);
            if (!statusExists) { return MethodResultDTO.NotFound("Room status not found"); }

            var typeExists = await _context.RoomTypes.AsNoTracking()
                .AnyAsync(t => t.IdRoomType == typeId, ct);
            if (!typeExists) { return MethodResultDTO.NotFound("Room type not found"); }

            var ownerUser = await _context.Users
                .Include(u => u.Person)
                .Where(u => u.Person.IdHotel == hotelId)
                .FirstOrDefaultAsync(ct);
            if (ownerUser is null)
            {
                return MethodResultDTO.Error("No user found for this hotel");
            }

            var room = new Room
            {
                Number = roomDTO.Number!,
                Capacity = roomDTO.Capacity ?? 0,
                Price = roomDTO.Price.HasValue ? (decimal)roomDTO.Price.Value : 0m,
                IdRoomType = typeId,
                IdRoomStatus = statusId,
                User = ownerUser
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Room created: room={RoomId}, number={Number}, hotel={HotelId}",
                room.IdRoom, room.Number, hotelId);

            return MethodResultDTO.Created("Created");
        }



        public async Task<MethodResultDTO> UpdateRoom(RoomDTO roomDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var room = await _context.Rooms
                .Include(r => r.User).ThenInclude(u => u.Person)
                .SingleOrDefaultAsync(r => r.IdRoom == roomDTO.IdRoom && r.User.Person.IdHotel == hotelId, ct);

            if (room is null)
            {
                _logger.LogWarning("UpdateRoom: room {RoomId} not found in hotel {HotelId}", roomDTO.IdRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            var numberExists = await _context.Rooms
                .AnyAsync(r => r.Number == roomDTO.Number
                               && r.IdRoom != roomDTO.IdRoom
                               && r.User.Person.IdHotel == hotelId, ct);
            if (numberExists)
            {
                return MethodResultDTO.BadRequest(
                    "Another room with this number already exists",
                    new Dictionary<string, List<string>>
                    {
                { "Number", new List<string> { "The room number already exists." } }
                    });
            }

            if (!int.TryParse(roomDTO.Status, out var statusId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid status identifier",
                    new Dictionary<string, List<string>> { { "Status", new List<string> { "Status must be a numeric id." } } });
            }
            if (!int.TryParse(roomDTO.Type, out var typeId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid type identifier",
                    new Dictionary<string, List<string>> { { "Type", new List<string> { "Type must be a numeric id." } } });
            }

            var statusExists = await _context.RoomStatuses.AsNoTracking()
                .AnyAsync(s => s.IdRoomStatus == statusId, ct);
            if (!statusExists) 
            { 
                return MethodResultDTO.NotFound("Room status not found"); 
            }

            var typeExists = await _context.RoomTypes.AsNoTracking()
                .AnyAsync(t => t.IdRoomType == typeId, ct);
            if (!typeExists) 
            { 
                return MethodResultDTO.NotFound("Room type not found"); 
            }


            room.Number = roomDTO.Number!;
            if (roomDTO.Capacity is not null) 
            { 
                room.Capacity = roomDTO.Capacity.Value; 
            }
            if (roomDTO.Price is not null) 
            { 
                room.Price = (decimal)roomDTO.Price.Value; 
            }
            room.IdRoomStatus = statusId;
            room.IdRoomType = typeId;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Room {RoomId} updated in hotel {HotelId}", room.IdRoom, hotelId);
            return MethodResultDTO.Ok("Updated");
        }


    }
}
