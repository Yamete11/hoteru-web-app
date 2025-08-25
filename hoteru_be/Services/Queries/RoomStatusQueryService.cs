using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Queries
{
    public class RoomStatusQueryService : IRoomStatusQueryService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<RoomStatusQueryService> _logger;

        public RoomStatusQueryService(MyDbContext context, ILogger<RoomStatusQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<List<StatusDTO>>> GetRoomStatuses(CancellationToken ct)
        {
            var list = await _context.RoomStatuses
                .AsNoTracking()
                .Select(x => new StatusDTO
                {
                    IdStatus = x.IdRoomStatus,
                    Title = x.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched {Count} room statuses", list.Count);
            return MethodResultDTO<List<StatusDTO>>.Ok(list, "Fetched");
        }
    }
}
