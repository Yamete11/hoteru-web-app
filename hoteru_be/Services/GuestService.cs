using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public class GuestService : IGuestService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GuestService> _logger;

        public GuestService(MyDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<GuestService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out var hotelId) ? hotelId : null;
        }

        public async Task<MethodResultDTO> DeleteGuest(int idPerson, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("DeleteGuest: unauthorized attempt for person {PersonId}", idPerson);
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var guest = await _context.Guests
                .Include(g => g.Person)
                .SingleOrDefaultAsync(g => g.IdPerson == idPerson && g.Person.IdHotel == hotelId, ct);

            if (guest is null)
            {
                _logger.LogWarning("DeleteGuest: guest {PersonId} not found in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.NotFound("Guest not found");
            }

            var hasReservations = await _context.Reservations
                 .AsNoTracking()
                 .AnyAsync(r => r.IdGuest == idPerson, ct);

            if (hasReservations)
            {
                _logger.LogWarning("DeleteGuest: guest {PersonId} has reservations; cannot delete", idPerson);
                return MethodResultDTO.Conflict("Guest has related reservations. Remove or reassign them first.");
            }

            try
            {
                _context.Guests.Remove(guest);
                _context.Persons.Remove(guest.Person);
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("DeleteGuest: guest {PersonId} deleted from hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteGuest: unexpected error for person {PersonId}", idPerson);
                return MethodResultDTO.Error("Unexpected error while deleting guest");
            }
        }

        public async Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string searchField, string searchQuery, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetGuests request");
                return new PaginatedResultDTO<GuestDTO>
                {
                    List = new List<GuestDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
            }

            var query = _context.Guests
                .AsNoTracking()
                .Where(g => g.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchField) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                var field = searchField.Trim().ToLower();
                var term = $"{searchQuery.Trim().ToLower()}%";

                switch (field)
                {
                    case "name":
                        query = query.Where(g => EF.Functions.Like(g.Person.Name.ToLower(), term));
                        break;
                    case "surname":
                        query = query.Where(g => EF.Functions.Like(g.Person.Surname.ToLower(), term));
                        break;
                    case "telnumber":
                        query = query.Where(g => g.TelNumber != null &&
                                                 EF.Functions.Like(g.TelNumber.ToLower(), term));
                        break;
                    case "email":
                        query = query.Where(g => EF.Functions.Like(g.Person.Email.ToLower(), term));
                        break;
                    default:
                        break;
                }
            }

            var total = await query.CountAsync(ct);

            var list = await query
                .OrderBy(g => g.IdPerson)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new GuestDTO
                {
                    IdPerson = x.IdPerson,
                    Name = x.Person.Name,
                    Surname = x.Person.Surname,
                    Email = x.Person.Email,
                    Passport = x.Passport,
                    TelNumber = x.TelNumber,
                    IdGuestStatus = x.GuestStatus.Title
                })
                .ToListAsync(ct);

            _logger.LogInformation("Fetched guests: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<GuestDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<MethodResultDTO<SpecificGuestDTO>> GetSpecificGuest(int idPerson, CancellationToken ct = default)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("GetSpecificGuest: unauthorized for person {PersonId}", idPerson);
                return MethodResultDTO<SpecificGuestDTO>.Unauthorized("Unauthorized");
            }

            try
            {
                var dto = await _context.Guests
                    .AsNoTracking()
                    .Where(g => g.IdPerson == idPerson && g.Person.IdHotel == hotelId)
                    .Select(g => new SpecificGuestDTO
                    {
                        IdPerson = g.IdPerson,
                        Name = g.Person.Name,
                        Surname = g.Person.Surname,
                        Email = g.Person.Email,
                        Passport = g.Passport,
                        TelNumber = g.TelNumber,
                        IdGuestStatus = g.IdGuestStatus
                    })
                    .FirstOrDefaultAsync(ct);

                if (dto is null)
                {
                    _logger.LogWarning("GetSpecificGuest: not found person {PersonId} in hotel {HotelId}", idPerson, hotelId);
                    return MethodResultDTO<SpecificGuestDTO>.NotFound("Guest not found");
                }

                _logger.LogInformation("GetSpecificGuest: fetched person {PersonId} for hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO<SpecificGuestDTO>.Ok(dto, "Fetched");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSpecificGuest canceled for person {PersonId}", idPerson);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSpecificGuest failed for person {PersonId}", idPerson);
                return MethodResultDTO<SpecificGuestDTO>.Error("Failed to fetch guest");
            }
        }

        public async Task<MethodResultDTO> PostGuest(GuestDTO guestDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("PostGuest: unauthorized");
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var errors = new Dictionary<string, List<string>>();

            
            var emailLower = guestDTO.Email.Trim().ToLowerInvariant();
            var tel = guestDTO.TelNumber.Trim();
            var passport = guestDTO.Passport.Trim();

            
            var emailExists = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.Person.IdHotel == hotelId && g.Person.Email.ToLower() == emailLower, ct);
            if (emailExists)
                errors.Add("Email", new() { "Another guest with this email already exists." });

            var telExists = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.TelNumber == tel, ct);
            if (telExists)
                errors.Add("TelNumber", new() { "Another guest with this tel. number already exists." });

            var passportExists = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.Passport == passport, ct);
            if (passportExists)
                errors.Add("Passport", new() { "Another guest with this passport already exists." });

           
            if (!int.TryParse(guestDTO.IdGuestStatus, out var statusId))
            {
                errors.Add("IdGuestStatus", new() { "Status must be a numeric id." });
            }
            else
            {
                var statusOk = await _context.GuestStatuses
                    .AsNoTracking()
                    .AnyAsync(s => s.IdGuestStatus == statusId, ct);
                if (!statusOk)
                    errors.Add("IdGuestStatus", new() { "Guest status not found." });
            }

            if (errors.Any())
            {
                _logger.LogInformation("PostGuest: validation failed for hotel {HotelId}", hotelId);
                return MethodResultDTO.Unprocessable("Validation failed", errors);
            }

            var person = new Person
            {
                Name = guestDTO.Name.Trim(),
                Surname = guestDTO.Surname.Trim(),
                Email = emailLower,
                IdHotel = hotelId.Value
            };

            var guest = new Guest
            {
                Passport = passport,
                TelNumber = tel,
                IdGuestStatus = statusId,
                Person = person
            };

            try
            {
                _context.Persons.Add(person);
                _context.Guests.Add(guest);
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("PostGuest: created person {PersonId} in hotel {HotelId}", guest.IdPerson, hotelId);
                return MethodResultDTO.Created("Created");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "PostGuest: DB error (hotel {HotelId})", hotelId);
                return MethodResultDTO.Error("Database error while creating guest");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PostGuest: unexpected error (hotel {HotelId})", hotelId);
                return MethodResultDTO.Error("Unexpected error while creating guest");
            }
        }



        public async Task<MethodResultDTO> UpdateGuest(GuestDTO guestDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("UpdateGuest: unauthorized for person {PersonId}", guestDTO.IdPerson);
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var guest = await _context.Guests
                .Include(g => g.Person)
                .FirstOrDefaultAsync(g => g.IdPerson == guestDTO.IdPerson && g.Person.IdHotel == hotelId, ct);

            if (guest is null)
            {
                _logger.LogWarning("UpdateGuest: not found person {PersonId} in hotel {HotelId}", guestDTO.IdPerson, hotelId);
                return MethodResultDTO.NotFound("Guest not found");
            }

            var errors = new Dictionary<string, List<string>>();

            var emailLower = guestDTO.Email?.Trim().ToLowerInvariant() ?? string.Empty;
            var tel = guestDTO.TelNumber?.Trim() ?? string.Empty;
            var passport = guestDTO.Passport?.Trim() ?? string.Empty;

            var passportConflict = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.Passport == passport && g.IdPerson != guestDTO.IdPerson, ct);
            if (passportConflict)
                errors["Passport"] = new() { "Another guest with this passport already exists." };

            var telConflict = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.TelNumber == tel && g.IdPerson != guestDTO.IdPerson, ct);
            if (telConflict)
                errors["TelNumber"] = new() { "Another guest with this tel. number already exists." };

            var emailConflict = await _context.Guests
                .AsNoTracking()
                .AnyAsync(g => g.Person.IdHotel == hotelId && g.Person.Email.ToLower() == emailLower && g.IdPerson != guestDTO.IdPerson, ct);
            if (emailConflict)
                errors["Email"] = new() { "Another guest with this email already exists." };

            if (!int.TryParse(guestDTO.IdGuestStatus, out var statusId))
            {
                errors["IdGuestStatus"] = new() { "Status must be a numeric id." };
            }
            else
            {
                var statusExists = await _context.GuestStatuses
                    .AsNoTracking()
                    .AnyAsync(s => s.IdGuestStatus == statusId, ct);
                if (!statusExists)
                    errors["IdGuestStatus"] = new() { "Guest status not found." };
            }

            if (errors.Any())
            {
                _logger.LogInformation("UpdateGuest: validation failed for person {PersonId}", guestDTO.IdPerson);
                return MethodResultDTO.Unprocessable("Validation failed", errors);
            }

            guest.TelNumber = tel;
            guest.Passport = passport;
            guest.IdGuestStatus = statusId;
            guest.Person.Name = guestDTO.Name?.Trim() ?? string.Empty;
            guest.Person.Surname = guestDTO.Surname?.Trim() ?? string.Empty;
            guest.Person.Email = emailLower;

            try
            {
                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("UpdateGuest: updated person {PersonId} in hotel {HotelId}", guestDTO.IdPerson, hotelId);
                return MethodResultDTO.Ok("Updated");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "UpdateGuest: DB error for person {PersonId}", guestDTO.IdPerson);
                return MethodResultDTO.Error("Database error while updating guest");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateGuest: unexpected error for person {PersonId}", guestDTO.IdPerson);
                return MethodResultDTO.Error("Unexpected error while updating guest");
            }
        }


    }
}
