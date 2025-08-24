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
    public class RoomTypeService : IRoomTypeService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<RoomTypeService> _logger;

        public RoomTypeService(MyDbContext context, ILogger<RoomTypeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<List<TypeDTO>>> GetRoomTypes(CancellationToken ct)
        {
            var list = await _context.RoomTypes
                .AsNoTracking()
                .Select(x => new TypeDTO
                {
                    IdType = x.IdRoomType,
                    Title = x.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched {Count} room types", list.Count);
            return MethodResultDTO<List<TypeDTO>>.Ok(list, "Fetched");
        }
    }
}
