using hoteru_be.Context;
using hoteru_be.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Queries
{
    public class ServiceQueryService : IServiceQueryService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IServiceQueryService> _logger;

        public ServiceQueryService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<ServiceQueryService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetServices request");
                return new PaginatedResultDTO<ServiceDTO> { List = new List<ServiceDTO>(), TotalCount = 0, Page = page, Limit = limit };
            }

            var query = _context.Services
                .AsNoTracking()
                .Where(s => s.User.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                var field = searchField.Trim().ToLower();
                var term = $"{searchQuery.Trim().ToLower()}%";

                switch (field)
                {
                    case "title":
                        query = query.Where(s => EF.Functions.Like(s.Title.ToLower(), term));
                        break;
                    case "description":
                        query = query.Where(s => s.Description != null && EF.Functions.Like(s.Description.ToLower(), term));
                        break;
                    case "sum":
                        if (TryParseMoney(searchQuery, out var sum))
                        {
                            sum = decimal.Round(sum, 2, MidpointRounding.AwayFromZero);
                            query = query.Where(s => s.Sum == sum);
                        }
                        else
                        {
                            query = query.Where(_ => false);
                        }
                        break;
                }
            }

            static bool TryParseMoney(string input, out decimal value)
            {
                var s = input.Trim();
                return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                    || decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
            }

            var total = await query.CountAsync(ct);

            var list = await query
                .OrderBy(s => s.IdService)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new ServiceDTO
                {
                    IdService = x.IdService,
                    Title = x.Title,
                    Sum = x.Sum,
                    Description = x.Description
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched services: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<ServiceDTO> { List = list, TotalCount = total, Page = page, Limit = limit };
        }

        public async Task<MethodResultDTO<ServiceDTO>> GetSpecificService(int idService, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("GetSpecificService unauthorized for service {ServiceId}", idService);
                return MethodResultDTO<ServiceDTO>.Unauthorized("HotelId claim missing");
            }

            var dto = await _context.Services
                .AsNoTracking()
                .Where(s => s.IdService == idService && s.User.Person.IdHotel == hotelId)
                .Select(s => new ServiceDTO
                {
                    IdService = s.IdService,
                    Title = s.Title,
                    Sum = s.Sum,
                    Description = s.Description
                })
                .FirstOrDefaultAsync(ct);

            if (dto is null)
            {
                _logger.LogWarning("GetSpecificService not found: service {ServiceId}, hotel {HotelId}", idService, hotelId);
                return MethodResultDTO<ServiceDTO>.NotFound("Service not found");
            }

            _logger.LogInformation("Fetched service {ServiceId} for hotel {HotelId}", idService, hotelId);
            return MethodResultDTO<ServiceDTO>.Ok(dto, "Fetched");
        }
    }
}
