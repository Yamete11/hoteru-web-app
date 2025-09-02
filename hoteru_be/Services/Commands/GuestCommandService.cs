using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Commands
{
    public class GuestCommandService : IGuestCommandService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<GuestCommandService> _logger;

        public GuestCommandService(MyDbContext context, ILogger<GuestCommandService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO> PostGuest(int hotelId, GuestDTO guestDTO, CancellationToken ct = default)
        {
            _logger.LogInformation("Attempting to add new guest for hotel {HotelId} with email {Email}", hotelId, guestDTO.Email);

            var errors = new Dictionary<string, List<string>>();

            var emailLower = guestDTO.Email.Trim().ToLowerInvariant();
            var tel = guestDTO.TelNumber.Trim();
            var passport = guestDTO.Passport.Trim();

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.Person.IdHotel == hotelId && g.Person.Email.ToLower() == emailLower, ct))
                errors.Add("Email", new() { "Email already exists." });

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.TelNumber == tel, ct))
                errors.Add("TelNumber", new() { "Tel number already exists." });

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.Passport == passport, ct))
                errors.Add("Passport", new() { "Passport already exists." });

            if (!int.TryParse(guestDTO.IdGuestStatus, out var statusId) ||
                !await _context.GuestStatuses.AsNoTracking().AnyAsync(s => s.IdGuestStatus == statusId, ct))
            {
                errors.Add("IdGuestStatus", new() { "Invalid guest status." });
            }

            if (errors.Any())
            {
                _logger.LogWarning("Validation failed while creating guest for hotel {HotelId}. Errors: {@Errors}", hotelId, errors);
                return MethodResultDTO.Unprocessable("Validation failed", errors);
            }

            var person = new Person
            {
                Name = guestDTO.Name.Trim(),
                Surname = guestDTO.Surname.Trim(),
                Email = emailLower,
                IdHotel = hotelId
            };

            var guest = new Guest
            {
                Passport = passport,
                TelNumber = tel,
                IdGuestStatus = statusId,
                Person = person
            };

            _context.Persons.Add(person);
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Guest {GuestId} created successfully in hotel {HotelId}", guest.IdPerson, hotelId);

            return MethodResultDTO.Created("Created");
        }

        public async Task<MethodResultDTO> UpdateGuest(int hotelId, GuestDTO guestDTO, CancellationToken ct = default)
        {
            _logger.LogInformation("Attempting to update guest {GuestId} in hotel {HotelId}", guestDTO.IdPerson, hotelId);

            var guest = await _context.Guests.Include(g => g.Person)
                .FirstOrDefaultAsync(g => g.IdPerson == guestDTO.IdPerson && g.Person.IdHotel == hotelId, ct);

            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found in hotel {HotelId}", guestDTO.IdPerson, hotelId);
                return MethodResultDTO.NotFound("Guest not found");
            }

            var errors = new Dictionary<string, List<string>>();

            var emailLower = guestDTO.Email.Trim().ToLowerInvariant();
            var tel = guestDTO.TelNumber.Trim();
            var passport = guestDTO.Passport.Trim();

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.Passport == passport && g.IdPerson != guestDTO.IdPerson, ct))
                errors.Add("Passport", new() { "Passport already exists." });

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.TelNumber == tel && g.IdPerson != guestDTO.IdPerson, ct))
                errors.Add("TelNumber", new() { "Tel number already exists." });

            if (await _context.Guests.AsNoTracking().AnyAsync(g => g.Person.IdHotel == hotelId && g.Person.Email.ToLower() == emailLower && g.IdPerson != guestDTO.IdPerson, ct))
                errors.Add("Email", new() { "Email already exists." });

            if (!int.TryParse(guestDTO.IdGuestStatus, out var statusId) ||
                !await _context.GuestStatuses.AsNoTracking().AnyAsync(s => s.IdGuestStatus == statusId, ct))
            {
                errors.Add("IdGuestStatus", new() { "Invalid guest status." });
            }

            if (errors.Any())
            {
                _logger.LogWarning("Validation failed while updating guest {GuestId} in hotel {HotelId}. Errors: {@Errors}", guestDTO.IdPerson, hotelId, errors);
                return MethodResultDTO.Unprocessable("Validation failed", errors);
            }

            guest.TelNumber = tel;
            guest.Passport = passport;
            guest.IdGuestStatus = statusId;
            guest.Person.Name = guestDTO.Name.Trim();
            guest.Person.Surname = guestDTO.Surname.Trim();
            guest.Person.Email = emailLower;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Guest {GuestId} updated successfully in hotel {HotelId}", guestDTO.IdPerson, hotelId);

            return MethodResultDTO.Ok("Updated");
        }

        public async Task<MethodResultDTO> DeleteGuest(int hotelId, int idPerson, CancellationToken ct = default)
        {
            _logger.LogInformation("Attempting to delete guest {GuestId} in hotel {HotelId}", idPerson, hotelId);

            var guest = await _context.Guests.Include(g => g.Person)
                .SingleOrDefaultAsync(g => g.IdPerson == idPerson && g.Person.IdHotel == hotelId, ct);

            if (guest is null)
            {
                _logger.LogWarning("Guest {GuestId} not found in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.NotFound("Guest not found");
            }

            if (await _context.Reservations.AsNoTracking().AnyAsync(r => r.IdGuest == idPerson, ct))
            {
                _logger.LogWarning("Cannot delete guest {GuestId} in hotel {HotelId} because they have reservations", idPerson, hotelId);
                return MethodResultDTO.Conflict("Guest has reservations.");
            }

            _context.Guests.Remove(guest);
            _context.Persons.Remove(guest.Person);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Guest {GuestId} deleted successfully from hotel {HotelId}", idPerson, hotelId);

            return MethodResultDTO.Ok("Deleted");
        }
    }
}
