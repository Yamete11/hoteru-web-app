using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class GuestService : IGuestService
    {

        private readonly MyDbContext _context;

        public GuestService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<MethodResultDTO> DeleteGuest(int IdPerson)
        {
            Person person = _context.Persons.SingleOrDefault(x => x.IdPerson == IdPerson);
            Guest guest = _context.Guests.SingleOrDefault(x => x.IdPerson == IdPerson);

            if (guest == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Not Found"
                };
            };

            _context.Guests.Remove(guest);
            _context.Persons.Remove(person);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<PaginatedResultDTO<GuestDTO>> GetGuests(int page, int limit)
        {

            var totalGuests = await _context.Guests.CountAsync();

            var guests = await _context.Guests
                .OrderBy(r => r.IdPerson)
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
            var guest = await _context.Guests
            .Include(r => r.GuestStatus)
            .Include(r => r.Person)
            .FirstOrDefaultAsync(x => x.IdPerson == IdPerson);

            return new SpecificGuestDTO
            {
                IdPerson = guest.IdPerson,
                Name = guest.Person.Name,
                Surname= guest.Person.Surname,
                Email= guest.Person.Email,
                Passport = guest.Passport,
                TelNumber = guest.TelNumber,
                IdGuestStatus = guest.IdGuestStatus
            };
        }

        public async Task<MethodResultDTO> PostGuest(GuestDTO guestDTO)
        {
            var hotel = await _context.Hotels.FirstOrDefaultAsync(r => r.IdHotel == 1);

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

        public async Task<MethodResultDTO> UpdateGuest(SpecificGuestDTO guestDTO)
        {
            var guest = await _context.Guests
                             .Include(r => r.GuestStatus)
                             .FirstOrDefaultAsync(r => r.IdPerson == guestDTO.IdPerson);

            var person = await _context.Persons.FirstOrDefaultAsync(r => r.IdPerson == guestDTO.IdPerson);

            if (guest == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Guest not found"
                };
            }

            /*var existingGuest = await _context.Guests
                .AnyAsync(r => r.Passport == guestDTO.Passport);

            if (existingGuest)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Another guest with this email already exists"
                };
            }*/

            guest.TelNumber = guestDTO.TelNumber;
            guest.Passport = guestDTO.Passport;
            guest.IdGuestStatus = guestDTO.IdGuestStatus;
            person.Name = guestDTO.Name;
            person.Surname = guestDTO.Surname;
            person.Email = guestDTO.Email;

            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };
        }
    }
}
