using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using System;

namespace hoteru_be.Services
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(MyDbContext context, IPasswordHasher<User> passwordHasher, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<MethodResultDTO> DeleteUser(int idPerson, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger?.LogWarning("DeleteUser unauthorized for person {PersonId}", idPerson);
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Reservations)
                .SingleOrDefaultAsync(u => u.IdPerson == idPerson && u.Person.IdHotel == hotelId, ct);

            if (user is null)
            {
                _logger?.LogWarning("User {PersonId} not found in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.NotFound("User not found");
            }

            var superAdminId = await _context.Users
                .AsNoTracking()
                .Where(u => u.IdUserType == 1)
                .Select(u => u.IdPerson)
                .FirstOrDefaultAsync(ct);

            if (superAdminId == 0)
            {
                _logger?.LogError("Super admin user not found");
                return MethodResultDTO.Error("Super admin user not found");
            }

            try
            {
                foreach (var r in user.Reservations)
                {
                    r.IdUser = superAdminId;
                }

                _context.Users.Remove(user);
                _context.Persons.Remove(user.Person);

                await _context.SaveChangesAsync(ct);

                _logger?.LogInformation("User {PersonId} deleted in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (OperationCanceledException)
            {
                _logger?.LogWarning("DeleteUser canceled for person {PersonId}", idPerson);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error deleting user {PersonId}", idPerson);
                return MethodResultDTO.Error("Unexpected error while deleting user.");
            }
        }


        public async Task<MethodResultDTO<FullUserDTO>> GetFullUser(int idUser, CancellationToken ct)
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


        public async Task<MethodResultDTO<UserDTO>> GetUser(string userName, CancellationToken ct)
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
                    new Dictionary<string, List<string>> {
                { "LoginName", new() { "Login name is required." } }
                    });
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


        public async Task<MethodResultDTO<List<ListUserDTO>>> GetUsers(CancellationToken ct)
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


        public async Task<MethodResultDTO> PostUser(NewUserDTO dto, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
                return MethodResultDTO.Unauthorized("HotelId claim missing");

            var login = dto.LoginName?.Trim();
            var email = dto.Email?.Trim();

            if (await _context.Users.AsNoTracking().AnyAsync(u => u.LoginName == login, ct))
            {
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "LoginName", new() { "Another user with this login already exists." } } });
            }

            if (await _context.Persons.AsNoTracking().AnyAsync(p => p.Email == email && p.IdHotel == hotelId, ct))
            {
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "Email", new() { "Another person with this email already exists." } } });
            }


            var userTypeExists = await _context.UserTypes.AsNoTracking()
                .AnyAsync(t => t.IdUserType == dto.IdUserType, ct);
            if (!userTypeExists)
                return MethodResultDTO.NotFound("User type not found");

            var person = new Person
            {
                Name = dto.Name.Trim(),
                Surname = dto.Surname.Trim(),
                Email = email!,
                IdHotel = hotelId.Value
            };

            var user = new User
            {
                LoginName = login!,
                IdUserType = dto.IdUserType,
                Person = person
            };
            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);
                _logger?.LogInformation("User created: personId={PersonId}, hotel={HotelId}", user.IdPerson, hotelId);
                return MethodResultDTO.Created("Created");
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating user {Login}", login);
                return MethodResultDTO.Error("An error occurred while creating user.");
            }
        }



        public async Task<MethodResultDTO> UpdateUser(UpdateUserDTO dto, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger?.LogWarning("UpdateUser unauthorized for person {PersonId}", dto.IdPerson);
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var login = (dto.LoginName ?? string.Empty).Trim();
            var email = (dto.Email ?? string.Empty).Trim();
            var name = (dto.Name ?? string.Empty).Trim();
            var surname = (dto.Surname ?? string.Empty).Trim();

            var user = await _context.Users
                .Include(u => u.Person)
                .SingleOrDefaultAsync(u => u.IdPerson == dto.IdPerson && u.Person.IdHotel == hotelId, ct);

            if (user is null)
            {
                _logger?.LogWarning("UpdateUser not found: person {PersonId}, hotel {HotelId}", dto.IdPerson, hotelId);
                return MethodResultDTO.NotFound("User not found");
            }

            var loginExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.LoginName == login && u.IdPerson != dto.IdPerson, ct);

            if (loginExists)
            {
                return MethodResultDTO.BadRequest(
                    "Validation failed",
                    new Dictionary<string, List<string>>
                    {
                { "LoginName", new() { "Another user with this login already exists." } }
                    });
            }

            var emailExists = await _context.Persons
                .AsNoTracking()
                .AnyAsync(p => p.Email == email && p.IdHotel == hotelId && p.IdPerson != dto.IdPerson, ct);

            if (emailExists)
            {
                return MethodResultDTO.BadRequest(
                    "Validation failed",
                    new Dictionary<string, List<string>>
                    {
                { "Email", new() { "Another person with this email already exists." } }
                    });
            }

            var userTypeExists = await _context.UserTypes
                .AsNoTracking()
                .AnyAsync(t => t.IdUserType == dto.IdUserType, ct);

            if (!userTypeExists)
            {
                return MethodResultDTO.NotFound("User type not found");
            }

            try
            {
                user.LoginName = login;
                user.IdUserType = dto.IdUserType;
                user.Person.Name = name;
                user.Person.Surname = surname;
                user.Person.Email = email;

                await _context.SaveChangesAsync(ct);

                _logger?.LogInformation("User {PersonId} updated in hotel {HotelId}", user.IdPerson, hotelId);
                return MethodResultDTO.Ok("Updated");
            }
            catch (OperationCanceledException)
            {
                _logger?.LogWarning("UpdateUser canceled for person {PersonId}", dto.IdPerson);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating user {PersonId}", dto.IdPerson);
                return MethodResultDTO.Error("An error occurred while updating user.");
            }
        }

    }
}
