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
    public class ReservationQueryService : IReservationQueryService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<ReservationQueryService> _logger;

        public ReservationQueryService(MyDbContext context, IHttpContextAccessor http, ILogger<ReservationQueryService> logger)
        {
            _context = context;
            _http = http;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var claim = _http.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetReservations request");
                return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill == null && r.Confirmed && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name": query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term)); break;
                    case "roomnumber": query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term)); break;
                    case "bookedby": query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term)); break;
                }
            }

            var total = await query.CountAsync(ct);
            var list = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new ReservationDTO
                {
                    IdReservation = r.IdReservation,
                    In = r.In.ToString("yyyy-MM-dd"),
                    Out = r.Out.ToString("yyyy-MM-dd"),
                    RoomNumber = r.Room.Number,
                    BookedBy = r.User.LoginName,
                    Name = r.Guest.Person.Name,
                    Surname = r.Guest.Person.Surname
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched reservations: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}", hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetHistory request");
                return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill != null && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name": query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term)); break;
                    case "roomnumber": query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term)); break;
                    case "bookedby": query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term)); break;
                }
            }

            var total = await query.CountAsync(ct);
            var list = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new ReservationDTO
                {
                    IdReservation = r.IdReservation,
                    In = r.In.ToString("yyyy-MM-dd"),
                    Out = r.Out.ToString("yyyy-MM-dd"),
                    RoomNumber = r.Room.Number,
                    BookedBy = r.User.LoginName,
                    Name = r.Guest.Person.Name,
                    Surname = r.Guest.Person.Surname
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched history: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}", hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetArrivals request");
                return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => !r.Confirmed && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name": query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term)); break;
                    case "roomnumber": query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term)); break;
                    case "bookedby": query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term)); break;
                }
            }

            var total = await query.CountAsync(ct);
            var list = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(r => new ReservationDTO
                {
                    IdReservation = r.IdReservation,
                    In = r.In.ToString("yyyy-MM-dd"),
                    Out = r.Out.ToString("yyyy-MM-dd"),
                    RoomNumber = r.Room.Number,
                    BookedBy = r.User.LoginName,
                    Name = r.Guest.Person.Name,
                    Surname = r.Guest.Person.Surname
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched arrivals: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}", hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<MethodResultDTO<FullReservationDTO>> GetSpecificHistory(int idReservation, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetSpecificHistory for reservation {ReservationId}", idReservation);
                return MethodResultDTO<FullReservationDTO>.Unauthorized("Unauthorized");
            }

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill != null && r.IdReservation == idReservation && r.Guest.Person.IdHotel == hotelId)
                .Include(r => r.Bill)
                .Include(r => r.Room).ThenInclude(rt => rt.RoomType)
                .Include(r => r.Deposit).ThenInclude(d => d.DepositType)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .FirstOrDefaultAsync(ct);

            if (reservation is null)
            {
                _logger.LogWarning("History not found: reservation {ReservationId}, hotel {HotelId}", idReservation, hotelId);
                return MethodResultDTO<FullReservationDTO>.NotFound("Reservation history not found");
            }

            var services = await _context.ReservationServices
                .AsNoTracking()
                .Where(rs => rs.IdReservation == idReservation)
                .Include(rs => rs.Service)
                .Select(rs => new ServiceHistoryDTO
                {
                    IdService = rs.IdService,
                    Title = rs.Service.Title,
                    Sum = rs.Service.Sum,
                    Date = rs.Date
                })
                .ToListAsync(ct);

            var dto = new FullReservationDTO
            {
                IdReservation = reservation.IdReservation,
                In = reservation.In.ToString("yyyy-MM-dd"),
                Out = reservation.Out.ToString("yyyy-MM-dd"),
                RoomNumber = reservation.Room.Number,
                RoomType = reservation.Room.RoomType.Title,
                BookedBy = reservation.User.LoginName,
                Name = reservation.Guest.Person.Name,
                Surname = reservation.Guest.Person.Surname,
                DepositSum = reservation.Deposit?.Sum ?? 0,
                DepositType = reservation.Deposit?.DepositType?.Title ?? string.Empty,
                BillSum = reservation.Bill!.Sum,
                Created = reservation.Bill.Created.ToString("yyyy-MM-dd"),
                Services = services
            };

            _logger.LogInformation("Fetched reservation history {ReservationId} for hotel {HotelId} with {ServicesCount} services", idReservation, hotelId, services.Count);

            return MethodResultDTO<FullReservationDTO>.Ok(dto, "Fetched");
        }

        public async Task<MethodResultDTO<ArrivalDTO>> GetSpecificArrival(int idArrival, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetSpecificArrival for reservation {ReservationId}", idArrival);
                return MethodResultDTO<ArrivalDTO>.Unauthorized("Unauthorized");
            }

            var services = await _context.ReservationServices
                .AsNoTracking()
                .Where(rs => rs.IdReservation == idArrival)
                .Include(rs => rs.Service)
                .Select(rs => new ServiceHistoryDTO
                {
                    IdService = rs.IdService,
                    Title = rs.Service.Title,
                    Sum = rs.Service.Sum,
                    Date = rs.Date
                })
                .ToListAsync(ct);

            var arrival = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.IdReservation == idArrival && r.Guest.Person.IdHotel == hotelId)
                .Select(r => new ArrivalDTO
                {
                    IdReservation = r.IdReservation,
                    In = r.In,
                    Out = r.Out,
                    Capacity = r.Capacity,
                    IdRoom = r.IdRoom,
                    IdDepositType = r.IdDeposit.HasValue ? r.Deposit.IdDepositType : 0,
                    IdGuest = r.IdGuest,
                    IdRoomType = r.Room.IdRoomType,
                    Services = services,
                    Confirmed = r.Confirmed,
                    DepositSum = r.IdDeposit.HasValue ? r.Deposit.Sum : 0,
                    Price = r.Price
                })
                .FirstOrDefaultAsync(ct);

            if (arrival is null)
            {
                _logger.LogWarning("Arrival not found: reservation {ReservationId}, hotel {HotelId}", idArrival, hotelId);
                return MethodResultDTO<ArrivalDTO>.NotFound("Arrival not found");
            }

            _logger.LogInformation("Fetched arrival {ReservationId} for hotel {HotelId} with {ServicesCount} services", idArrival, hotelId, services.Count);
            return MethodResultDTO<ArrivalDTO>.Ok(arrival, "Fetched");
        }
    }
}
