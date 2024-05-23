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
    public class HotelService : IHotelService
    {
        private readonly MyDbContext _context;
        private readonly IEmailService _emailService;

        public HotelService(MyDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<MethodResultDTO> PostHotel(HotelDTO hotelDTO)
        {
            var errors = new Dictionary<string, List<string>>();

            var existingEmail = await _context.Persons
                .AnyAsync(p => p.Email == hotelDTO.Email);
            if (existingEmail)
            {
                errors.Add("Email", new List<string> { "Another person with this email already exists." });
            }

            var existingLoginName = await _context.Users
                .AnyAsync(u => u.LoginName == hotelDTO.LoginName);
            if (existingLoginName)
            {
                errors.Add("LoginName", new List<string> { "Another user with this login name already exists." });
            }

            var existingTitle = await _context.Hotels
                .AnyAsync(h => h.Title == hotelDTO.Title);
            if (existingTitle)
            {
                errors.Add("Title", new List<string> { "Another hotel with this title already exists." });
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


            Address address = new Address
            {
                Country = hotelDTO.Country,
                City = hotelDTO.City,
                Street = hotelDTO.Street,
                Postcode = hotelDTO.Postcode
            };

            _context.Addresses.Add(address);

            Hotel hotel = new Hotel
            {
                Title = hotelDTO.Title,
                Address = address
            };

            Person person = new Person
            {
                Name = hotelDTO.Name,
                Surname = hotelDTO.Surname,
                Email = hotelDTO.Email,
                Hotel = hotel
            };
            User user = new User
            {
                LoginName = hotelDTO.LoginName,
                Password = hotelDTO.Password,
                Person = person,
                IdUserType = 1,
            };

            _context.Hotels.Add(hotel);
            _context.Persons.Add(person);
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(hotelDTO.Email, "Welcome to HOTERU",
                $"You have created your own hotel {hotelDTO.Title}\n\n" +
                $"Your login: {hotelDTO.LoginName}\n" +
                $"Your password: {hotelDTO.Password} \n\n" +
                "Good luck!!");

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }

    }
}
