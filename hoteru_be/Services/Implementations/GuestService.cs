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

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }

        public async Task<IEnumerable<GuestDTO>> GetGuests()
        {
            return await _context.Guests
                .Include(g => g.Person)
                .Include(g => g.GuestStatus)
                .Select(x => new GuestDTO
                {
                    IdPerson = x.IdPerson,
                    Name = x.Person.Name, 
                    Surname = x.Person.Surname,
                    Email = x.Person.Email,
                    Passport = x.Passport,
                    TelNumber = x.TelNumber,
                    IdGuestStatus = x.GuestStatus.Title 
                }).ToListAsync();
        }


    }
}
