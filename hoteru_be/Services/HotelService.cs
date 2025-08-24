using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services;
using Microsoft.AspNetCore.Identity;
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
    public class HotelService : IHotelService
    {
        private readonly MyDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<HotelService> _logger;

        public HotelService(MyDbContext context, IEmailService emailService, ILogger<HotelService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<MethodResultDTO> DeleteHotel(string hotelTitle, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(hotelTitle))
                return MethodResultDTO.BadRequest("Hotel title is required.");

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Title == hotelTitle);

            if (hotel is null)
            {
                _logger.LogWarning("DeleteHotel: not found '{HotelTitle}'", hotelTitle);
                return MethodResultDTO.NotFound($"Hotel with title '{hotelTitle}' not found.");
            }

            try
            {
                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Hotel '{HotelTitle}' deleted", hotelTitle);
                return MethodResultDTO.Ok($"Hotel '{hotelTitle}' deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting hotel '{HotelTitle}'", hotelTitle);
                return MethodResultDTO.Error("Unexpected error while deleting hotel.");
            }
        }



        public async Task<MethodResultDTO> PostHotel(HotelDTO hotelDTO, CancellationToken ct)
        {
            var errors = new Dictionary<string, List<string>>();

            if (await _context.Persons.AnyAsync(p => p.Email == hotelDTO.Email, ct))
                (errors["Email"] = new List<string>()).Add("Another person with this email already exists.");

            if (await _context.Users.AnyAsync(u => u.LoginName == hotelDTO.LoginName, ct))
                (errors["LoginName"] = new List<string>()).Add("Another user with this login name already exists.");

            if (await _context.Hotels.AnyAsync(h => h.Title == hotelDTO.Title, ct))
                (errors["Title"] = new List<string>()).Add("Another hotel with this title already exists.");

            if (errors.Any())
                return MethodResultDTO.BadRequest("Validation failed", errors);

            var adminTypeId = await _context.UserTypes
                .AsNoTracking()
                .Where(t => t.Title == "Admin")
                .Select(t => t.IdUserType)
                .FirstOrDefaultAsync(ct);
            if (adminTypeId == 0)
            {
                adminTypeId = await _context.UserTypes
                    .AsNoTracking()
                    .OrderBy(t => t.IdUserType)
                    .Select(t => t.IdUserType)
                    .FirstAsync(ct);
            }

            var address = new Address
            {
                Country = hotelDTO.Country,
                City = hotelDTO.City,
                Street = hotelDTO.Street,
                Postcode = hotelDTO.Postcode
            };
            _context.Addresses.Add(address);

            var hotel = new Hotel
            {
                Title = hotelDTO.Title,
                Address = address
            };
            _context.Hotels.Add(hotel);

            var person = new Person
            {
                Name = hotelDTO.Name,
                Surname = hotelDTO.Surname,
                Email = hotelDTO.Email,
                Hotel = hotel
            };
            _context.Persons.Add(person);

            var user = new User
            {
                LoginName = hotelDTO.LoginName,
                Person = person,
                IdUserType = adminTypeId
            };
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, hotelDTO.Password);
            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync(ct);

                try
                {
                    await _emailService.SendEmailAsync(
                        hotelDTO.Email,
                        "Welcome to HOTERU",
                        $"You have created your own hotel {hotelDTO.Title}\n\n" +
                        $"Your login: {hotelDTO.LoginName}\n" +
                        $"Your password: {hotelDTO.Password}\n\n" +
                        "Good luck!!",
                        ct);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Welcome email failed for hotel '{Title}' to '{Email}'", hotelDTO.Title, hotelDTO.Email);
                }

                _logger?.LogInformation("Hotel created: title='{Title}', login='{Login}'", hotelDTO.Title, hotelDTO.LoginName);
                return MethodResultDTO.Created("Created");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error creating hotel '{Title}'", hotelDTO.Title);
                return MethodResultDTO.Error("Unexpected error while creating hotel.");
            }
        }


    }
}
