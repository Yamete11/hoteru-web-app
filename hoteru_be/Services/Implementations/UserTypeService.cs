using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class UserTypeService : IUserTypeService
    {
        private readonly MyDbContext _context;

        public UserTypeService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TypeDTO>> GetUserTypes()
        {
            return await _context.UserTypes.Select(x => new TypeDTO
            {
                IdType = x.IdUserType,
                Title = x.Title,
            }).ToListAsync();
        }
    }
}
