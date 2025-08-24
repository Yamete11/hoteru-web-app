using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services
{
    public class DepositTypeService : IDepositTypeService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<DepositTypeService> _logger;

        public DepositTypeService(MyDbContext context, ILogger<DepositTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<List<TypeDTO>>> GetDepositTypes(CancellationToken ct)
        {
            var list = await _context.DepositTypes
                .AsNoTracking()
                .Select(x => new TypeDTO
                {
                    IdType = x.IdDepositType,
                    Title = x.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched {Count} deposit types", list.Count);
            return MethodResultDTO<List<TypeDTO>>.Ok(list, "Fetched");
        }
    }
}
