using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Queries
{
    public class RoomQueryService : IRoomQueryService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<RoomQueryService> _logger;

        public RoomQueryService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<RoomQueryService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out var hotelId) ? hotelId : null;
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

            query = idRoom != 0
                ? query.Where(x => x.IdRoom == idRoom || x.IdRoomStatus == readyStatusId)
                : query.Where(x => x.IdRoomStatus == readyStatusId);

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
                return new PaginatedResultDTO<RoomDTO> { List = new List<RoomDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Rooms
                .AsNoTracking()
                .Where(r => r.User.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var field = (searchField ?? "").Trim().ToLower();
                var term = $"{searchQuery.Trim().ToLower()}%";

                query = field switch
                {
                    "number" => query.Where(r => EF.Functions.Like(r.Number.ToLower(), term)),
                    "capacity" => int.TryParse(searchQuery, out var cap) ? query.Where(r => r.Capacity == cap) : query,
                    "type" => query.Where(r => EF.Functions.Like(r.RoomType.Title.ToLower(), term)),
                    "status" => query.Where(r => EF.Functions.Like(r.RoomStatus.Title.ToLower(), term)),
                    _ => query
                };
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

            return new PaginatedResultDTO<RoomDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<MethodResultDTO<SpecificRoomDTO>> GetSpecificRoom(int idRoom, CancellationToken ct = default)
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
    }
}
