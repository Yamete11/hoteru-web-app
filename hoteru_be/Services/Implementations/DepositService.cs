using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class DepositService : IDepositService
    {
        private readonly MyDbContext _context;

        public DepositService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<DepositDTO> GetDeposit(int IdDeposit)
        {
            return await _context.Deposits
                .Where(x => x.IdDeposit == IdDeposit)
                .Select(x => new DepositDTO
                {
                    IdDeposit = x.IdDeposit,
                    Sum = x.Sum,
                    IdDepositType = x.IdDepositType
                }).FirstOrDefaultAsync(); 
        }

    }
}
