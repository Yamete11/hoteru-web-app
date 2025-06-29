using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace hoteru_be.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly MyDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(MyDbContext context, IPasswordHasher<User> passwordHasher, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<MethodResultDTO> DeleteUser(int IdPerson)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();

            var person = await _context.Persons.Include(p => p.Hotel)
                                               .FirstOrDefaultAsync(p => p.IdPerson == IdPerson && p.IdHotel == hotelId);
            var user = await _context.Users.Include(u => u.Reservations)
                                           .Include(u => u.Person)
                                           .FirstOrDefaultAsync(u => u.IdPerson == IdPerson && u.Person.IdHotel == hotelId);

            if (user == null || person == null)
                return NotFound("User or Person not found");

            var superAdminUser = await _context.Users.Include(u => u.UserType)
                                                     .FirstOrDefaultAsync(u => u.UserType.IdUserType == 1);

            if (superAdminUser == null)
                return Error("Super admin user not found");

            foreach (var reservation in user.Reservations)
            {
                reservation.IdUser = superAdminUser.IdPerson;
            }

            _context.Users.Remove(user);
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return Success("Deleted");
        }

        public async Task<FullUserDTO> GetFullUser(int idUser)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return null;

            return await _context.Users
                .Where(x => x.IdPerson == idUser && x.Person.IdHotel == hotelId)
                .Include(r => r.Person)
                .ThenInclude(gr => gr.Hotel)
                .Select(r => new FullUserDTO
                {
                    Name = r.Person.Name,
                    Surname = r.Person.Surname,
                    Email = r.Person.Email,
                    LoginName = r.LoginName,
                    IdUserType = r.IdUserType
                }).FirstOrDefaultAsync();
        }

        public async Task<UserDTO> GetUser(string userName)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return null;

            return await _context.Users
                .Where(x => x.LoginName == userName && x.Person.IdHotel == hotelId)
                .Include(r => r.Person)
                .ThenInclude(gr => gr.Hotel)
                .Select(r => new UserDTO
                {
                    LoginName = r.LoginName,
                    IdUser = r.IdPerson,
                    CompanyTitle = r.Person.Hotel.Title,
                }).FirstOrDefaultAsync();
        }

        public async Task<List<ListUserDTO>> GetUsers()
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return new List<ListUserDTO>();

            return await _context.Users
                .Where(u => u.Person.IdHotel == hotelId)
                .Include(r => r.UserType)
                .Select(r => new ListUserDTO
                {
                    IdPerson = r.IdPerson,
                    LoginName = r.LoginName,
                    UserType = r.UserType.Title
                }).ToListAsync();
        }

        public async Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();

            var existingUser = await _context.Users
                .AnyAsync(r => r.LoginName == newUserDTO.LoginName);

            if (existingUser)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = new Dictionary<string, List<string>>
                    {
                        { "LoginName", new List<string> { "Another guest with this Login already exists." } }
                    }
                };
            }

            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.IdHotel == hotelId);
            if (hotel == null) return Error("Hotel not found");

            var person = new Person
            {
                Name = newUserDTO.Name,
                Surname = newUserDTO.Surname,
                Email = newUserDTO.Email,
                Hotel = hotel
            };

            var user = new User
            {
                LoginName = newUserDTO.LoginName,
                IdUserType = newUserDTO.IdUserType,
                Person = person,
                Password = _passwordHasher.HashPassword(null, newUserDTO.Password)
            };

            _context.Persons.Add(person);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Success("Created");
        }

        public async Task<MethodResultDTO> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return ForbiddenResult();

            var user = await _context.Users
                .Include(r => r.UserType)
                .Include(r => r.Person)
                .FirstOrDefaultAsync(r => r.IdPerson == updateUserDTO.IdPerson && r.Person.IdHotel == hotelId);

            if (user == null)
                return NotFound("User not found");

            var existingUser = await _context.Users
                .AnyAsync(r => r.LoginName == updateUserDTO.LoginName && r.IdPerson != updateUserDTO.IdPerson);

            if (existingUser)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = new Dictionary<string, List<string>>
                    {
                        { "LoginName", new List<string> { "Another guest with this Login already exists." } }
                    }
                };
            }

            user.LoginName = updateUserDTO.LoginName;
            user.IdUserType = updateUserDTO.IdUserType;
            user.Person.Name = updateUserDTO.Name;
            user.Person.Surname = updateUserDTO.Surname;
            user.Person.Email = updateUserDTO.Email;

            await _context.SaveChangesAsync();

            return Success("Updated");
        }

        private MethodResultDTO ForbiddenResult() => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.Forbidden,
            Message = "Hotel ID missing or invalid"
        };

        private MethodResultDTO NotFound(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.NotFound,
            Message = message
        };

        private MethodResultDTO Error(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            Message = message
        };

        private MethodResultDTO Success(string message) => new MethodResultDTO
        {
            HttpStatusCode = HttpStatusCode.OK,
            Message = message
        };
    }
}
