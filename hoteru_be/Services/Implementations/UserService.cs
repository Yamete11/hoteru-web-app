using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
    }
}
