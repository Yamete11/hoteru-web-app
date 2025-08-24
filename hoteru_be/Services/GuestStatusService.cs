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
    public class GuestStatusService : IGuestStatusService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<GuestStatusService> _logger;

        public GuestStatusService(MyDbContext context, ILogger<GuestStatusService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<List<StatusDTO>>> GetGuestStatuses(CancellationToken ct)
        {
            var list = await _context.GuestStatuses
                .AsNoTracking()
                .Select(x => new StatusDTO
                {
                    IdStatus = x.IdGuestStatus,
                    Title = x.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched {Count} guest statuses", list.Count);
            return MethodResultDTO<List<StatusDTO>>.Ok(list, "Fetched");
        }
    }
}
