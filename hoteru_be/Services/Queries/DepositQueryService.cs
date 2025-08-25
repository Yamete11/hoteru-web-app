using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Queries
{
    public class DepositQueryService : IDepositQueryService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<DepositQueryService> _logger;

        public DepositQueryService(MyDbContext context, ILogger<DepositQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<DepositDTO>> GetDeposit(int idDeposit, CancellationToken ct)
        {
            var dto = await _context.Deposits
                .AsNoTracking()
                .Where(x => x.IdDeposit == idDeposit)
                .Select(x => new DepositDTO
                {
                    IdDeposit = x.IdDeposit,
                    Sum = x.Sum,
                    IdDepositType = x.IdDepositType
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                _logger.LogWarning("Deposit not found: {DepositId}", idDeposit);
                return MethodResultDTO<DepositDTO>.NotFound("Deposit not found");
            }

            _logger.LogInformation("Fetched deposit {DepositId}", idDeposit);
            return MethodResultDTO<DepositDTO>.Ok(dto, "Fetched");
        }

    }
}
