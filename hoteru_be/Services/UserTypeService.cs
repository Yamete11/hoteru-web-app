using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services
{
    public class UserTypeService : IUserTypeService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<UserTypeService> _logger;

        public UserTypeService(MyDbContext context, ILogger<UserTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<List<TypeDTO>>> GetUserTypes(CancellationToken ct)
        {
            var list = await _context.UserTypes
                .AsNoTracking()
                .Select(x => new TypeDTO
                {
                    IdType = x.IdUserType,
                    Title = x.Title,
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched {Count} user types", list.Count);
            return MethodResultDTO<List<TypeDTO>>.Ok(list, "Fetched");
        }
    }
}
