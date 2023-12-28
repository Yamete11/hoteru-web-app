using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(r => r.IdHotel == 1);

            Person person = new Person
            {
                Name = newUserDTO.Name,
                Surname = newUserDTO.Surname,
                Email = newUserDTO.Email,
                Hotel = hotel
            };

            User user = new User
            {
                LoginName = newUserDTO.Login,
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
    }
}
