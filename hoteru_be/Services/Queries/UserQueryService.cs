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
    public class UserQueryService : IUserQueryService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserQueryService> _logger;

        public UserQueryService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserQueryService> logger)
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

        public async Task<MethodResultDTO<FullUserDTO>> GetFullUser(int idUser, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger?.LogWarning("GetFullUser unauthorized for user {UserId}", idUser);
                return MethodResultDTO<FullUserDTO>.Unauthorized("HotelId claim missing");
            }

            var dto = await _context.Users
                .AsNoTracking()
                .Where(u => u.IdPerson == idUser && u.Person.IdHotel == hotelId)
                .Select(u => new FullUserDTO
                {
                    Name = u.Person.Name,
                    Surname = u.Person.Surname,
                    Email = u.Person.Email,
                    LoginName = u.LoginName,
                    IdUserType = u.IdUserType
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                _logger?.LogWarning("FullUser not found: user {UserId}, hotel {HotelId}", idUser, hotelId);
                return MethodResultDTO<FullUserDTO>.NotFound("User not found");
            }

            _logger?.LogInformation("Fetched full user {UserId} for hotel {HotelId}", idUser, hotelId);
            return MethodResultDTO<FullUserDTO>.Ok(dto, "Fetched");
        }

        public async Task<MethodResultDTO<UserDTO>> GetUser(string userName, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger?.LogWarning("GetUser unauthorized for login {UserName}", userName);
                return MethodResultDTO<UserDTO>.Unauthorized("HotelId claim missing");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                return MethodResultDTO<UserDTO>.BadRequest(
                    "Validation failed",
                    new Dictionary<string, List<string>> { { "LoginName", new() { "Login name is required." } } });
            }

            var dto = await _context.Users
                .AsNoTracking()
                .Where(u => u.LoginName == userName && u.Person.IdHotel == hotelId)
                .Select(u => new UserDTO
                {
                    LoginName = u.LoginName,
                    IdUser = u.IdPerson,
                    CompanyTitle = u.Person.Hotel.Title
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                _logger?.LogWarning("User not found: login {UserName}, hotel {HotelId}", userName, hotelId);
                return MethodResultDTO<UserDTO>.NotFound("User not found");
            }

            _logger?.LogInformation("Fetched user '{UserName}' (id={UserId}) for hotel {HotelId}", dto.LoginName, dto.IdUser, hotelId);
            return MethodResultDTO<UserDTO>.Ok(dto, "Fetched");
        }

        public async Task<MethodResultDTO<List<ListUserDTO>>> GetUsers(CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger?.LogWarning("User unauthorized");
                return MethodResultDTO<List<ListUserDTO>>.Unauthorized("User unauthorized");
            }

            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.Person.IdHotel == hotelId)
                .OrderBy(u => u.LoginName)
                .Select(u => new ListUserDTO
                {
                    IdPerson = u.IdPerson,
                    LoginName = u.LoginName,
                    UserType = u.UserType.Title
                })
                .ToListAsync(ct);

            _logger?.LogInformation("Fetched {Count} users for hotel {HotelId}", users.Count, hotelId);
            return MethodResultDTO<List<ListUserDTO>>.Ok(users, "Fetched");
        }
    }
}
