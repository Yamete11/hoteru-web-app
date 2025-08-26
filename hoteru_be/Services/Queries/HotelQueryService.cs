using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Queries
{
    public class HotelQueryService : IHotelQueryService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<HotelQueryService> _logger;

        public HotelQueryService(MyDbContext context, ILogger<HotelQueryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO<HotelDTO>> GetHotel(int hotelId, CancellationToken ct = default)
        {
            try
            {
                var dto = await _context.Hotels
                    .AsNoTracking()
                    .Include(h => h.Address)
                    .Where(h => h.IdHotel == hotelId)
                    .Select(h => new HotelDTO
                    {
                        Title = h.Title,
                        City = h.Address.City,
                        Country = h.Address.Country,
                        Street = h.Address.Street,
                        Postcode = h.Address.Postcode
                    })
                    .FirstOrDefaultAsync(ct);

                if (dto is null)
                {
                    return MethodResultDTO<HotelDTO>.NotFound("Hotel not found");
                }
                   

                return MethodResultDTO<HotelDTO>.Ok(dto, "Fetched");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetHotel failed for HotelId={HotelId}", hotelId);
                return MethodResultDTO<HotelDTO>.Error("Unexpected error");
            }
        }
    }
}
