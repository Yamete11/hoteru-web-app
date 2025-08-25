using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Queries
{
    public class GuestQueryService : IGuestQueryService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GuestQueryService> _logger;

        public GuestQueryService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<GuestQueryService> logger)
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

        public async Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string? searchQuery = null, string? searchField = null, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetGuests request");
                return new PaginatedResultDTO<GuestDTO> { List = new List<GuestDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Guests
                .AsNoTracking()
                .Where(g => g.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                var field = searchField.Trim().ToLower();
                var term = $"{searchQuery.Trim().ToLower()}%";

                query = field switch
                {
                    "name" => query.Where(g => EF.Functions.Like(g.Person.Name.ToLower(), term)),
                    "surname" => query.Where(g => EF.Functions.Like(g.Person.Surname.ToLower(), term)),
                    "telnumber" => query.Where(g => g.TelNumber != null && EF.Functions.Like(g.TelNumber.ToLower(), term)),
                    "email" => query.Where(g => EF.Functions.Like(g.Person.Email.ToLower(), term)),
                    _ => query
                };
            }

            var total = await query.CountAsync(ct);

            var list = await query
                .OrderBy(g => g.IdPerson)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new GuestDTO
                {
                    IdPerson = x.IdPerson,
                    Name = x.Person.Name,
                    Surname = x.Person.Surname,
                    Email = x.Person.Email,
                    Passport = x.Passport,
                    TelNumber = x.TelNumber,
                    IdGuestStatus = x.GuestStatus.Title
                })
                .ToListAsync(ct);

            return new PaginatedResultDTO<GuestDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<MethodResultDTO<SpecificGuestDTO>> GetSpecificGuest(int idPerson, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetSpecificGuest request");
                return MethodResultDTO<SpecificGuestDTO>.Unauthorized("Unauthorized");
            }

            var dto = await _context.Guests
                .AsNoTracking()
                .Where(g => g.IdPerson == idPerson && g.Person.IdHotel == hotelId)
                .Select(g => new SpecificGuestDTO
                {
                    IdPerson = g.IdPerson,
                    Name = g.Person.Name,
                    Surname = g.Person.Surname,
                    Email = g.Person.Email,
                    Passport = g.Passport,
                    TelNumber = g.TelNumber,
                    IdGuestStatus = g.IdGuestStatus
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                return MethodResultDTO<SpecificGuestDTO>.NotFound("Guest not found");
            }

            return MethodResultDTO<SpecificGuestDTO>.Ok(dto, "Fetched");
        }
    }
}
