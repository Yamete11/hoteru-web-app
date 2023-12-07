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
    public class ReservationService : IReservationService
    {
        private readonly MyDbContext _context;

        public ReservationService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReservationDTO>> GetReservations()
        {
            return await _context.Reservations
            .Where(r => r.Bill == null && r.Confirmed == true)
            .Include(r => r.Room)
            .Include(r => r.User)
            .ThenInclude(u => u.Person)
            .Include(r => r.GuestReservations)
            .ThenInclude(gr => gr.Guest)
            .ThenInclude(g => g.Person)
            .Select(r => new ReservationDTO
            {
                IdReservation = r.IdReservation,
                In = r.In.ToString("yyyy-MM-dd"),
                Out = r.Out.ToString("yyyy-MM-dd"),
                RoomNumber = r.Room.Number,
                BookedBy = r.User.LoginName,
                Name = r.User.Person.Name,
                Surname = r.User.Person.Surname
            }
            ).ToListAsync();

        }

        public async Task<IEnumerable<ReservationDTO>> GetHistory()
        {
            return await _context.Reservations
            .Where(r => r.Bill != null)
            .Include(r => r.Room)
            .Include(r => r.User)
            .ThenInclude(u => u.Person)
            .Include(r => r.GuestReservations)
            .ThenInclude(gr => gr.Guest)
            .ThenInclude(g => g.Person)
            .Select(r => new ReservationDTO
            {
                IdReservation = r.IdReservation,
                In = r.In.ToString("yyyy-MM-dd"),
                Out = r.Out.ToString("yyyy-MM-dd"),
                RoomNumber = r.Room.Number,
                BookedBy = r.User.LoginName,
                Name = r.User.Person.Name,
                Surname = r.User.Person.Surname
            }
            ).ToListAsync();
        }

        public async Task<IEnumerable<ReservationDTO>> GetArrivals()
        {
            return await _context.Reservations
             .Where(r => r.Confirmed == false)
             .Include(r => r.Room)
             .Include(r => r.User)
             .ThenInclude(u => u.Person)
             .Include(r => r.GuestReservations)
             .ThenInclude(gr => gr.Guest)
             .ThenInclude(g => g.Person)
             .Select(r => new ReservationDTO
              {
                 IdReservation = r.IdReservation,
                 In = r.In.ToString("yyyy-MM-dd"),
                 Out = r.Out.ToString("yyyy-MM-dd"),
                 RoomNumber = r.Room.Number,
                 BookedBy = r.User.LoginName,
                 Name = r.User.Person.Name,
                 Surname = r.User.Person.Surname
              }).ToListAsync();
        }

        public async Task<IEnumerable<FullReservationDTO>> GetSpecificHistory(int IdReservation)
        {

            var services = await _context.ReservationService
                .Where(r => r.IdReservation == IdReservation)
                .Include(r => r.Service)
                .Select(r => new ServiceHistoryDTO
                {
                    IdService = r.IdService,
                    Title = r.Service.Title,
                    Sum = r.Service.Sum,
                    Date = r.Date.ToString("yyyy-MM-dd")
                }).ToListAsync();

            return await _context.Reservations
            .Where(r => r.Bill != null && r.IdReservation == IdReservation)
            .Include(r => r.Bill)
            .Include(r => r.Room)
            .ThenInclude(u => u.RoomType)
            .Include(r => r.Deposit)
            .ThenInclude(u => u.DepositType)
            .Include(r => r.ReservationServices)
            .ThenInclude(u => u.Service)
            .Include(r => r.User)
            .ThenInclude(u => u.Person)
            .Include(r => r.GuestReservations)
            .ThenInclude(gr => gr.Guest)
            .ThenInclude(g => g.Person)
            .Select(r => new FullReservationDTO
            {
                IdReservation = r.IdReservation,
                In = r.In.ToString("yyyy-MM-dd"),
                Out = r.Out.ToString("yyyy-MM-dd"),
                RoomNumber = r.Room.Number,
                RoomType = r.Room.RoomType.Title,
                BookedBy = r.User.LoginName,
                Name = r.User.Person.Name,
                Surname = r.User.Person.Surname,
                DepositSum = r.Deposit.Sum,
                DepositType = r.Deposit.DepositType.Title,
                BillSum = r.Bill.Sum,
                Created = r.Bill.Created.ToString("yyyy-MM-dd"),
                Services = services
            }).ToListAsync();
        }

        public async Task<MethodResultDTO> DeleteSpecificReservation(int IdReservation)
        {
            Reservation reservation = _context.Reservations.SingleOrDefault(x => x.IdReservation == IdReservation);

            if (reservation == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Not Found"
                };
            };

            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }
    }
}
