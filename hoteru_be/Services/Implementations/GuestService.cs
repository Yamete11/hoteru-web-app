using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class GuestService : IGuestService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GuestService(MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out var hotelId) ? hotelId : 0;
        }

        public async Task<MethodResultDTO> DeleteGuest(int IdPerson)
        {
            var person = await _context.Persons.SingleOrDefaultAsync(x => x.IdPerson == IdPerson);
            var guest = await _context.Guests.SingleOrDefaultAsync(x => x.IdPerson == IdPerson);

            if (guest == null || person == null || person.IdHotel != GetHotelIdFromToken())
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Not Found"
                };
            }

            _context.Guests.Remove(guest);
            _context.Persons.Remove(person);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit, string searchQuery = null, string searchField = null)
        {
            int hotelId = GetHotelIdFromToken();

            IQueryable<Guest> query = _context.Guests
                .Include(g => g.Person)
                .Include(g => g.GuestStatus)
                .Where(g => g.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                searchQuery = searchQuery.ToLower();
                switch (searchField.ToLower())
                {
                    case "name":
                        query = query.Where(g => g.Person.Name.ToLower().StartsWith(searchQuery));
                        break;
                    case "surname":
                        query = query.Where(g => g.Person.Surname.ToLower().StartsWith(searchQuery));
                        break;
                    case "telnumber":
                        query = query.Where(g => g.TelNumber.ToLower().StartsWith(searchQuery));
                        break;
                    case "email":
                        query = query.Where(g => g.Person.Email.ToLower().StartsWith(searchQuery));
                        break;
                }
            }

            var totalGuests = await query.CountAsync();

            var guests = await query
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
                .ToListAsync();

            return new PaginatedResultDTO<GuestDTO>
            {
                List = guests,
                TotalCount = totalGuests,
                Page = page,
                Limit = limit
            };
        }

        public async Task<SpecificGuestDTO> GetSpecificGuest(int IdPerson)
        {
            int hotelId = GetHotelIdFromToken();

            var guest = await _context.Guests
                .Include(r => r.GuestStatus)
                .Include(r => r.Person)
                .FirstOrDefaultAsync(x => x.IdPerson == IdPerson && x.Person.IdHotel == hotelId);

            if (guest == null)
            {
                return null;
            }

            return new SpecificGuestDTO
            {
                IdPerson = guest.IdPerson,
                Name = guest.Person.Name,
                Surname = guest.Person.Surname,
                Email = guest.Person.Email,
                Passport = guest.Passport,
                TelNumber = guest.TelNumber,
                IdGuestStatus = guest.IdGuestStatus
            };
        }

        public async Task<MethodResultDTO> PostGuest(GuestDTO guestDTO)
        {
            int hotelId = GetHotelIdFromToken();
            var errors = new Dictionary<string, List<string>>();

            var existingEmailGuest = await _context.Guests
                .Include(r => r.Person)
                .AnyAsync(r => r.Person.Email == guestDTO.Email && r.Person.IdHotel == hotelId);

            if (existingEmailGuest)
                errors.Add("Email", new List<string> { "Another guest with this email already exists." });

            var existingTelNumberGuest = await _context.Guests
                .AnyAsync(r => r.TelNumber == guestDTO.TelNumber);
            if (existingTelNumberGuest)
                errors.Add("TelNumber", new List<string> { "Another guest with this tel. number already exists." });

            var existingPassportGuest = await _context.Guests
                .AnyAsync(r => r.Passport == guestDTO.Passport);
            if (existingPassportGuest)
                errors.Add("Passport", new List<string> { "Another guest with this passport already exists." });

            if (errors.Any())
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = errors
                };
            }

            var hotel = await _context.Hotels.FindAsync(hotelId);

            Person person = new Person
            {
                Name = guestDTO.Name,
                Surname = guestDTO.Surname,
                Email = guestDTO.Email,
                Hotel = hotel
            };

            Guest guest = new Guest
            {
                Passport = guestDTO.Passport,
                TelNumber = guestDTO.TelNumber,
                IdGuestStatus = int.Parse(guestDTO.IdGuestStatus),
                Person = person
            };

            _context.Persons.Add(person);
            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }


        public async Task<MethodResultDTO> UpdateGuest(GuestDTO guestDTO)
        {
            int hotelId = GetHotelIdFromToken();

            var guest = await _context.Guests
                .Include(r => r.Person)
                .FirstOrDefaultAsync(r => r.IdPerson == guestDTO.IdPerson && r.Person.IdHotel == hotelId);

            if (guest == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Guest not found"
                };
            }

            var errors = new Dictionary<string, List<string>>();

            var passportConflict = await _context.Guests
                .AnyAsync(g => g.Passport == guestDTO.Passport && g.IdPerson != guestDTO.IdPerson);
            if (passportConflict)
            {
                errors["Passport"] = new List<string> { "Another guest with this passport already exists." };
            }

            var telConflict = await _context.Guests
                .AnyAsync(g => g.TelNumber == guestDTO.TelNumber && g.IdPerson != guestDTO.IdPerson);
            if (telConflict)
            {
                errors["TelNumber"] = new List<string> { "Another guest with this tel. number already exists." };
            }

            var emailConflict = await _context.Guests
                .Include(g => g.Person)
                .AnyAsync(g => g.Person.Email == guestDTO.Email && g.IdPerson != guestDTO.IdPerson && g.Person.IdHotel == hotelId);
            if (emailConflict)
            {
                errors["Email"] = new List<string> { "Another guest with this email already exists." };
            }

            if (errors.Any())
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Validation failed",
                    Errors = errors
                };
            }

            guest.TelNumber = guestDTO.TelNumber;
            guest.Passport = guestDTO.Passport;
            guest.IdGuestStatus = int.Parse(guestDTO.IdGuestStatus);
            guest.Person.Name = guestDTO.Name;
            guest.Person.Surname = guestDTO.Surname;
            guest.Person.Email = guestDTO.Email;

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };
        }

    }
}
