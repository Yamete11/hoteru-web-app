using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using System.Collections.Generic;
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

            _context.Hotels.Add(hotel);


            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(hotelDTO.Email, "Welcom to HOTERU", 
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
