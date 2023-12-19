using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class DepositTypeService : IDepositTypeService
    {

        private readonly MyDbContext _context;

        public DepositTypeService(MyDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TypeDTO>> GetDepositTypes()
        {
            return await _context.DepositTypes.Select(x => new TypeDTO
            {
                IdType = x.IdDepositType,
                Title = x.Title,
            }).ToListAsync();
        }
    }
}
