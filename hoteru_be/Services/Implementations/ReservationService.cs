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
            .ThenInclude(gr => gr.Guest)
            .ThenInclude(g => g.Person)
            .Select(r => new ReservationDTO
            {
                IdReservation = r.IdReservation,
                In = r.In.ToString("yyyy-MM-dd"),
                Out = r.Out.ToString("yyyy-MM-dd"),
                RoomNumber = r.Room.Number,
                BookedBy = r.User.LoginName,
                Name = r.Guest.Person.Name,
                Surname = r.Guest.Person.Surname
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
                .Where(r => r.Bill != null)
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Include(r => r.Room)
                .Include(r => r.User)
                .Include(r => r.Guest)
                    .ThenInclude(g => g.Person)
                .Select(r => new ReservationDTO
                {
                    IdReservation = r.IdReservation,
                    In = r.In.ToString("yyyy-MM-dd"),
                    Out = r.Out.ToString("yyyy-MM-dd"),
                    RoomNumber = r.Room.Number,
                    BookedBy = r.User.LoginName,
                    Name = r.Guest.Person.Name,
                    Surname = r.Guest.Person.Surname
                })
                .ToListAsync();

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
            .ThenInclude(gr => gr.Guest)
            .ThenInclude(g => g.Person)
            .Select(r => new ReservationDTO
            {
                IdReservation = r.IdReservation,
                In = r.In.ToString("yyyy-MM-dd"),
                Out = r.Out.ToString("yyyy-MM-dd"),
                RoomNumber = r.Room.Number,
                BookedBy = r.User.LoginName,
                Name = r.Guest.Person.Name,
                Surname = r.Guest.Person.Surname
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

        public async Task<FullReservationDTO> GetSpecificHistory(int IdReservation)
        {
            var services = await _context.ReservationServices
                .Where(r => r.IdReservation == IdReservation)
                .Include(r => r.Service)
                .Select(r => new ServiceHistoryDTO
                {
                    IdService = r.IdService,
                    Title = r.Service.Title,
                    Sum = r.Service.Sum,
                    Date = r.Date.ToString("yyyy-MM-dd")
                }).ToListAsync();

            var reservation = await _context.Reservations
                .Where(r => r.Bill != null && r.IdReservation == IdReservation)
                .Include(r => r.Bill)
                .Include(r => r.Room)
                    .ThenInclude(rt => rt.RoomType)
                .Include(r => r.Deposit)
                    .ThenInclude(d => d.DepositType)
                .Include(r => r.Guest)
                    .ThenInclude(g => g.Person)
                .Include(r => r.User)
                    .ThenInclude(u => u.Person)
                .FirstOrDefaultAsync();

            if (reservation == null)
                return null;

            return new FullReservationDTO
            {
                IdReservation = reservation.IdReservation,
                In = reservation.In.ToString("yyyy-MM-dd"),
                Out = reservation.Out.ToString("yyyy-MM-dd"),
                RoomNumber = reservation.Room.Number,
                RoomType = reservation.Room.RoomType.Title,
                BookedBy = reservation.User.LoginName,
                Name = reservation.Guest.Person.Name,
                Surname = reservation.Guest.Person.Surname,
                DepositSum = reservation.IdDeposit.HasValue ? reservation.Deposit.Sum : 0,
                DepositType = reservation.IdDeposit.HasValue ? reservation.Deposit.DepositType.Title : "",
                BillSum = reservation.Bill.Sum,
                Created = reservation.Bill.Created.ToString("yyyy-MM-dd"),
            };
        }


        public async Task<MethodResultDTO> DeleteSpecificReservation(int idReservation)
        {
            var reservation = await _context.Reservations
                .SingleOrDefaultAsync(x => x.IdReservation == idReservation);

            if (reservation == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Reservation not found"
                };
            }


            var reservationServices = await _context.ReservationServices
                .Where(x => x.IdReservation == idReservation)
                .ToListAsync();

            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.IdRoom == reservation.IdRoom);
            room.IdRoomStatus = 1; 

           
            _context.ReservationServices.RemoveRange(reservationServices);
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Reservation deleted successfully"
            };
        }


        public async Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.IdRoom == reservationDTO.IdRoom);
            if (room == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Room not found"
                };
            } else if(room.IdRoomStatus != 3)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Room is occupied"
                };
            }

            Deposit deposit = null;


            if (reservationDTO.IdDepositType != 0)
            {
                deposit = new Deposit
                {
                    Sum = reservationDTO.Sum,
                    IdDepositType = reservationDTO.IdDepositType
                };
                _context.Deposits.Add(deposit);
            }

            var reservation = new Reservation
            {
                Capacity = reservationDTO.Capacity,
                Price = reservationDTO.Price,
                In = reservationDTO.In,
                Out = reservationDTO.Out,
                Confirmed = reservationDTO.Confirmed,
                IdRoom = room.IdRoom,
                IdUser = reservationDTO.IdUser,
                IdGuest = reservationDTO.IdPerson,
                Deposit = deposit
            };

            _context.Reservations.Add(reservation);


            var guest = await _context.Guests.SingleOrDefaultAsync(x => x.IdPerson == reservationDTO.IdPerson);
            if (guest == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Guest not found"
                };
            }


            foreach (var serviceDTO in reservationDTO.Services)
            {
                var service = await _context.Services.SingleOrDefaultAsync(x => x.IdService == serviceDTO.IdService);
                if (service != null)
                {
                    var reservationService = new Entities.ReservationService
                    {
                        Reservation = reservation,
                        Service = service
                    };
                    _context.ReservationServices.Add(reservationService);
                }
            }
            room.IdRoomStatus = 2;
            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Created"
            };
        }


        public async Task<ArrivalDTO> GetSpecificArrival(int IdArrival)
        {

            var services = await _context.ReservationServices
                .Where(r => r.IdReservation == IdArrival)
                .Include(r => r.Service)
                .Select(r => new ServiceHistoryDTO
                {
                    IdService = r.IdService,
                    Title = r.Service.Title,
                    Sum = r.Service.Sum,
                    Date = r.Date.ToString("yyyy-MM-dd")
                }).ToListAsync();


            return await _context.Reservations
                .Where(r => r.IdReservation == IdArrival)
                .Include(r => r.Room)
                .Include(r => r.Deposit)
                .Select(r => new ArrivalDTO
                {
                   IdReservation = r.IdReservation,
                   In = r.In,
                   Out = r.Out,
                   Capacity = r.Capacity,
                   IdRoom = r.IdRoom,
                   IdDepositType = r.IdDeposit.HasValue ? r.Deposit.IdDepositType : 0,
                   IdGuest = r.IdGuest,
                   IdRoomType = r.Room.IdRoomType,
                   Services = services,
                   Confirmed = r.Confirmed,
                   DepositSum = r.IdDeposit.HasValue ? r.Deposit.Sum : 0,
                   Price = r.Price
                }).FirstOrDefaultAsync();
        }

        public async Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO)
        {
            var reservation = await _context.Reservations.SingleOrDefaultAsync(r => r.IdReservation == arrivalDTO.IdReservation);

            var deposit = await _context.Deposits.SingleOrDefaultAsync(r => r.IdDeposit == reservation.IdDeposit);


            if(deposit == null && arrivalDTO.IdDepositType != 0)
            {
                var depo = new Deposit
                {
                    Sum = arrivalDTO.Price,
                    IdDepositType = arrivalDTO.IdDepositType
                };
                _context.Deposits.Add(depo);
                reservation.Deposit = depo;
            } 
            else if(deposit != null && arrivalDTO.IdDepositType == 0)
            {
                _context.Deposits.Remove(deposit);
            } 
            else if(deposit != null && arrivalDTO.IdDepositType != 0)
            {
                deposit.IdDepositType = arrivalDTO.IdDepositType;
                deposit.Sum = arrivalDTO.DepositSum;
            }
           

            reservation.IdGuest = arrivalDTO.IdGuest;
            

            reservation.In = arrivalDTO.In;
            reservation.Out = arrivalDTO.Out;
            reservation.Capacity = arrivalDTO.Capacity;
            reservation.IdRoom = arrivalDTO.IdRoom;
            reservation.Price = arrivalDTO.Price;

            var services = await _context.ReservationServices.Where(r => r.IdReservation == arrivalDTO.IdReservation).ToListAsync();

            var arrivalServiceIds = arrivalDTO.Services.Select(s => s.IdService).ToList();

            _context.ReservationServices.RemoveRange(
                services.Where(s => !arrivalServiceIds.Contains(s.IdService)));

            foreach (var serviceId in arrivalServiceIds)
            {
                if (!services.Any(s => s.IdService == serviceId))
                {
                    _context.ReservationServices.Add(new Entities.ReservationService
                    {
                        IdReservation = arrivalDTO.IdReservation,
                        IdService = serviceId,
                        Date = DateTime.Now
                    });
                }
            }


            await _context.SaveChangesAsync();
            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };
        }

        public async Task<MethodResultDTO> ConfirmReservation(int IdReservation)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Guest)
                    .ThenInclude(g => g.Person)
                .Include(r => r.Room)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.IdReservation == IdReservation);

            if (reservation == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Reservation not found"
                };
            }

            var room = reservation.Room;

            if (reservation.Confirmed == false)
            {
                reservation.Confirmed = true;
                room.IdRoomStatus = 2;
            }
            else if (reservation.Confirmed == true)
            {
                var bill = new Bill
                {
                    Created = DateTime.Now,
                    Sum = reservation.Price,

                    InDate = reservation.In,
                    OutDate = reservation.Out,

                    GuestName = reservation.Guest?.Person?.Name ?? "Unknown",
                    GuestSurname = reservation.Guest?.Person?.Surname ?? "Unknown",

                    RoomNumber = room.Number,
                    BookedBy = reservation.User?.LoginName ?? "Unknown"
                };

                reservation.Bill = bill;
                room.IdRoomStatus = 1;
            }

            await _context.SaveChangesAsync();

            return new MethodResultDTO
            {
                HttpStatusCode = HttpStatusCode.OK,
                Message = "Confirmed"
            };
        }

    }
}
