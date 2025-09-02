using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public class ServiceCommandService : IServiceCommandService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ServiceCommandService> _logger;

        public ServiceCommandService(MyDbContext context, ILogger<ServiceCommandService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO> PostService(int hotelId, ServiceDTO serviceDTO, CancellationToken ct = default)
        {

            var title = serviceDTO.Title?.Trim() ?? string.Empty;
            var exists = await _context.Services
                .AsNoTracking()
                .AnyAsync(s => s.User.Person.IdHotel == hotelId && s.Title.ToLower() == title.ToLower(), ct);

            if (exists)
            {
                return MethodResultDTO.BadRequest(
                    "Service with this title already exists.",
                    new Dictionary<string, List<string>> { { "Title", new() { "Title already exists." } } });
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

        public async Task<MethodResultDTO> UpdateService(int hotelId, ServiceDTO serviceDTO, CancellationToken ct = default)
        {
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

        public async Task<MethodResultDTO> DeleteService(int hotelId, int idService, CancellationToken ct = default)
        {
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
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting service {ServiceId} in hotel {HotelId}", idService, hotelId);
                return MethodResultDTO.Error("Unexpected error while deleting service.");
            }
        }
    }
}
