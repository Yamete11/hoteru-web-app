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
    public class UserService : IUserService
    {

        private readonly MyDbContext _context;

        public UserService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<MethodResultDTO> DeleteUser(int IdPerson)
        {
            var person = await _context.Persons.SingleOrDefaultAsync(x => x.IdPerson == IdPerson);
            var user = await _context.Users.Include(u => u.Reservations).SingleOrDefaultAsync(x => x.IdPerson == IdPerson);

            if (user == null || person == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "User or Person not found"
                };
            }

            var superAdminUser = await _context.Users.FirstOrDefaultAsync(u => u.UserType.IdUserType == 1);

            if (superAdminUser == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "Super admin user not found"
                };
            }

            foreach (var reservation in user.Reservations)
            {
                reservation.IdUser = superAdminUser.IdPerson;
            }

            await _context.SaveChangesAsync();

            _context.Users.Remove(user);
            _context.Persons.Remove(person);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }


        public async Task<FullUserDTO> GetFullUser(int idUser)
        {
            return await _context.Users
            .Where(x => x.IdPerson == idUser)
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
            return await _context.Users
            .Where(x => x.LoginName == userName)
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
            return await _context.Users
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
            var hotel = await _context.Hotels.FirstOrDefaultAsync(r => r.IdHotel == 1);

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

            Person person = new Person
            {
                Name = newUserDTO.Name,
                Surname = newUserDTO.Surname,
                Email = newUserDTO.Email,
                Hotel = hotel
            };

            User user = new User
            {
                LoginName = newUserDTO.LoginName,
                Password = newUserDTO.Password,
                IdUserType = newUserDTO.IdUserType,
                Person = person
            };


            _context.Persons.Add(person);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }

        public async Task<MethodResultDTO> UpdateUser(UpdateUserDTO updateUserDTO)
        {
            var user = await _context.Users
                                .Include(r => r.UserType)
                                .FirstOrDefaultAsync(r => r.IdPerson == updateUserDTO.IdPerson);

            var person = await _context.Persons
                                .FirstOrDefaultAsync(r => r.IdPerson == updateUserDTO.IdPerson);

            if (user == null || person == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "User not found"
                };
            }

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
            person.Name = updateUserDTO.Name;
            person.Surname = updateUserDTO.Surname;
            person.Email = updateUserDTO.Email;

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };
        }

    }
}
