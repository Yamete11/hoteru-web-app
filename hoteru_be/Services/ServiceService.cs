using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public class ServiceService : IServiceService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<ServiceService> logger)
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

        public async Task<MethodResultDTO> DeleteService(int idService, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("DeleteService unauthorized for service {ServiceId}", idService);
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var svc = await _context.Services
                .AsNoTracking()
                .Where(s => s.IdService == idService && s.User.Person.IdHotel == hotelId)
                .Select(s => new { s.IdService })
                .FirstOrDefaultAsync(ct);

            if (svc is null)
            {
                _logger.LogWarning("DeleteService not found: service {ServiceId}, hotel {HotelId}", idService, hotelId);
                return MethodResultDTO.NotFound("Service not found");
            }

            try
            {
                var stub = new Service { IdService = svc.IdService };
                _context.Entry(stub).State = EntityState.Deleted;

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Service {ServiceId} deleted for hotel {HotelId}", idService, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DeleteService conflict for {ServiceId} in hotel {HotelId}", idService, hotelId);
                return MethodResultDTO.Conflict("Cannot delete service due to related data.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DeleteService canceled for service {ServiceId}", idService);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting service {ServiceId} in hotel {HotelId}", idService, hotelId);
                return MethodResultDTO.Error("Unexpected error while deleting service.");
            }
        }

        public async Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetServices request");
                return new PaginatedResultDTO<ServiceDTO>
                {
                    List = new List<ServiceDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
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
                        query = query.Where(s => s.Description != null &&
                                                 EF.Functions.Like(s.Description.ToLower(), term));
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


                    default:

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

            return new PaginatedResultDTO<ServiceDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<MethodResultDTO<ServiceDTO>> GetSpecificService(int idService, CancellationToken ct)
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

        public async Task<MethodResultDTO> PostService(ServiceDTO serviceDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("PostService unauthorized");
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var title = serviceDTO.Title?.Trim() ?? string.Empty;
            var exists = await _context.Services
                .AsNoTracking()
                .AnyAsync(s => s.User.Person.IdHotel == hotelId &&
                               s.Title.ToLower() == title.ToLower(), ct);

            if (exists)
            {
                return MethodResultDTO.BadRequest(
                    "Service with this title already exists.",
                    new Dictionary<string, List<string>> {
                        { "Title", new List<string> { "Title already exists." } }
                    });
            }

            var user = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Person.IdHotel == hotelId, ct);

            if (user is null)
            {
                _logger.LogError("PostService: no user found for hotel {HotelId}", hotelId);
                return MethodResultDTO.Error("No user found for this hotel.");
            }

            var service = new Service
            {
                Title = title,
                Sum = serviceDTO.Sum ?? 0,
                Description = serviceDTO.Description?.Trim(),
                User = user
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Service created: id={ServiceId}, hotel={HotelId}", service.IdService, hotelId);
            return MethodResultDTO.Created("Created");
        }

        public async Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("UpdateService unauthorized for service {ServiceId}", serviceDTO.IdService);
                return MethodResultDTO.Unauthorized("HotelId claim missing");
            }

            var service = await _context.Services
                .Where(s => s.IdService == serviceDTO.IdService && s.User.Person.IdHotel == hotelId)
                .FirstOrDefaultAsync(ct);

            if (service is null)
            {
                _logger.LogWarning("UpdateService not found: service {ServiceId}, hotel {HotelId}", serviceDTO.IdService, hotelId);
                return MethodResultDTO.NotFound("Service not found");
            }

            service.Title = serviceDTO.Title;
            if (serviceDTO.Sum is not null) service.Sum = serviceDTO.Sum.Value;
            if (serviceDTO.Description is not null) service.Description = serviceDTO.Description;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Service {ServiceId} updated for hotel {HotelId}", service.IdService, hotelId);
            return MethodResultDTO.Ok("Updated");
        }


    }
}
