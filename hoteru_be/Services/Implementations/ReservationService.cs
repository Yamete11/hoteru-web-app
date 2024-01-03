using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit)
        {

            var totalReservations = await _context.Reservations
                .Where(r => r.Bill == null && r.Confirmed == true)
                .CountAsync();

            var reservations = await _context.Reservations
            .OrderBy(r => r.IdRoom)
            .Skip((page - 1) * limit)
            .Take(limit)
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
                Name = r.GuestReservations.FirstOrDefault().Guest.Person.Name,
                Surname = r.GuestReservations.FirstOrDefault().Guest.Person.Surname
            }
            ).ToListAsync();

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = reservations,
                TotalCount = totalReservations,
                Page = page,
                Limit = limit
            };

        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit)
        {
            var totalReservations = await _context.Reservations
                .Where(r => r.Bill != null)
                .CountAsync();

            var reservations = await _context.Reservations
            .OrderBy(r => r.IdRoom)
            .Skip((page - 1) * limit)
            .Take(limit)
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
                Name = r.GuestReservations.FirstOrDefault().Guest.Person.Name,
                Surname = r.GuestReservations.FirstOrDefault().Guest.Person.Surname
            }
            ).ToListAsync();

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = reservations,
                TotalCount = totalReservations,
                Page = page,
                Limit = limit
            };
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit)
        {
            var totalReservations = await _context.Reservations
                .Where(r => r.Confirmed == false)
                .CountAsync();

            var reservations = await _context.Reservations
            .OrderBy(r => r.IdRoom)
            .Skip((page - 1) * limit)
            .Take(limit)
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
                Name = r.GuestReservations.FirstOrDefault().Guest.Person.Name,
                Surname = r.GuestReservations.FirstOrDefault().Guest.Person.Surname
            }
            ).ToListAsync();

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = reservations,
                TotalCount = totalReservations,
                Page = page,
                Limit = limit
            };
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
                Name = r.GuestReservations.FirstOrDefault().Guest.Person.Name,
                Surname = r.GuestReservations.FirstOrDefault().Guest.Person.Surname,
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

        public async Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO)
        {
            Deposit deposit = new Deposit
            {
                Sum = reservationDTO.Sum,
                IdDepositType = reservationDTO.IdDepositType
            };
            _context.Deposits.Add(deposit);


            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.IdRoom == reservationDTO.IdRoom);
            Reservation reservation = new Reservation
            {
                Capacity = reservationDTO.Capacity,
                Price = reservationDTO.Price,
                In = reservationDTO.In,
                Out = reservationDTO.Out,
                Confirmed = reservationDTO.Confirmed,
                IdRoom = room.IdRoom,
                IdUser = reservationDTO.IdUser,
                Deposit = deposit
                
            };

            _context.Reservations.Add(reservation);

    

            var guest = await _context.Guests.SingleOrDefaultAsync(x => x.IdPerson == reservationDTO.IdPerson);
            GuestReservation guestReservation = new GuestReservation
            {
                 Reservation = reservation,
                 Guest = guest
            };

            _context.GuestReservations.Add(guestReservation);


            if (reservationDTO.Services.Count > 0)
            {
                for (int i = 0; i < reservationDTO.Services.Count; i++)
                {
                    var service = await _context.Services.SingleOrDefaultAsync(x => x.IdService == reservationDTO.Services[i].IdService);
                    Entities.ReservationService reservationService = new Entities.ReservationService
                    {
                        Reservation = reservation,
                        Service = service
                    };
                    _context.ReservationService.Add(reservationService);
                }
            }



            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }

        public async Task<ArrivalDTO> GetSpecificArrival(int IdArrival)
        {

            var services = await _context.ReservationService
                .Where(r => r.IdReservation == IdArrival)
                .Include(r => r.Service)
                .Select(r => new ServiceHistoryDTO
                {
                    IdService = r.IdService,
                    Title = r.Service.Title,
                    Sum = r.Service.Sum,
                    Date = r.Date.ToString("yyyy-MM-dd")
                }).ToListAsync();

            var guest = await _context.GuestReservations.SingleOrDefaultAsync(r => r.IdReservation == IdArrival);

            return await _context.Reservations
                .Where(r => r.IdReservation == IdArrival && r.Confirmed == false)
                .Include(r => r.Room)
                .Select(r => new ArrivalDTO
                {
                   IdReservation = r.IdReservation,
                   In = r.In.ToString("yyyy-MM-dd"),
                   Out = r.Out.ToString("yyyy-MM-dd"),
                   Capacity = r.Capacity,
                   IdRoom = r.IdRoom,
                   IdDeposit = r.IdDeposit.HasValue ? r.IdDeposit.Value : 0,
                   IdGuest = guest.IdGuest,
                   IdRoomType = r.Room.IdRoomType,
                   Services = services

                }).FirstOrDefaultAsync();
        }
    }
}
