using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using hoteru_be.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationService(MyDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit, string searchQuery = "", string searchField = "")
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };

            var query = _context.Reservations
                .Where(r => r.Bill == null && r.Confirmed)
                .Include(r => r.Room)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Where(r => r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                switch (searchField.ToLower())
                {
                    case "name":
                        query = query.Where(r => r.Guest.Person.Name.ToLower().StartsWith(searchQuery));
                        break;
                    case "roomnumber":
                        query = query.Where(r => r.Room.Number.ToLower().StartsWith(searchQuery));
                        break;
                    case "bookedby":
                        query = query.Where(r => r.User.LoginName.ToLower().StartsWith(searchQuery));
                        break;
                }
            }

            var totalReservations = await query.CountAsync();

            var reservations = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
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

        public async Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit, string searchQuery = "", string searchField = "")
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };

            var query = _context.Reservations
                .Where(r => r.Bill != null)
                .Include(r => r.Room)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Where(r => r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                switch (searchField.ToLower())
                {
                    case "name":
                        query = query.Where(r => r.Guest.Person.Name.ToLower().StartsWith(searchQuery));
                        break;
                    case "roomnumber":
                        query = query.Where(r => r.Room.Number.ToLower().StartsWith(searchQuery));
                        break;
                    case "bookedby":
                        query = query.Where(r => r.User.LoginName.ToLower().StartsWith(searchQuery));
                        break;
                }
            }

            var totalReservations = await query.CountAsync();

            var reservations = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
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

        public async Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit, string searchQuery = "", string searchField = "")
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return new PaginatedResultDTO<ReservationDTO> { List = new List<ReservationDTO>(), TotalCount = 0, Page = page, Limit = limit };

            var query = _context.Reservations
                .Where(r => r.Confirmed == false)
                .Include(r => r.Room)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Where(r => r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                switch (searchField.ToLower())
                {
                    case "name":
                        query = query.Where(r => r.Guest.Person.Name.ToLower().StartsWith(searchQuery));
                        break;
                    case "roomnumber":
                        query = query.Where(r => r.Room.Number.ToLower().StartsWith(searchQuery));
                        break;
                    case "bookedby":
                        query = query.Where(r => r.User.LoginName.ToLower().StartsWith(searchQuery));
                        break;
                }
            }

            var totalReservations = await query.CountAsync();

            var reservations = await query
                .OrderBy(r => r.IdRoom)
                .Skip((page - 1) * limit)
                .Take(limit)
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

        public async Task<FullReservationDTO> GetSpecificHistory(int IdReservation)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return null;

            var reservation = await _context.Reservations
                .Where(r => r.Bill != null && r.IdReservation == IdReservation && r.Guest.Person.IdHotel == hotelId)
                .Include(r => r.Bill)
                .Include(r => r.Room).ThenInclude(rt => rt.RoomType)
                .Include(r => r.Deposit).ThenInclude(d => d.DepositType)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .FirstOrDefaultAsync();

            if (reservation == null) return null;

            var services = await _context.ReservationServices
                .Where(rs => rs.IdReservation == IdReservation)
                .Include(rs => rs.Service)
                .Select(rs => new ServiceHistoryDTO
                {
                    IdService = rs.IdService,
                    Title = rs.Service.Title,
                    Sum = rs.Service.Sum,
                    Date = rs.Date.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

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
                DepositSum = reservation.Deposit?.Sum ?? 0,
                DepositType = reservation.Deposit?.DepositType?.Title ?? "",
                BillSum = reservation.Bill.Sum,
                Created = reservation.Bill.Created.ToString("yyyy-MM-dd"),
                Services = services
            };
        }

        public async Task<MethodResultDTO> DeleteSpecificReservation(int idReservation)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null)
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                };

            var reservation = await _context.Reservations
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .SingleOrDefaultAsync(x => x.IdReservation == idReservation && x.Guest.Person.IdHotel == hotelId);

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
            if (room != null)
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
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null)
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                };

            var room = await _context.Rooms
                .Include(r => r.User).ThenInclude(u => u.Person)
                .SingleOrDefaultAsync(r => r.IdRoom == reservationDTO.IdRoom && r.User.Person.IdHotel == hotelId);

            if (room == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Room not found"
                };
            }
            if (room.IdRoomStatus != 3)
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

            var guest = await _context.Guests
                .Include(g => g.Person)
                .SingleOrDefaultAsync(g => g.IdPerson == reservationDTO.IdPerson && g.Person.IdHotel == hotelId);

            if (guest == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.BadRequest,
                    Message = "Guest not found"
                };
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

            foreach (var serviceDTO in reservationDTO.Services)
            {
                var service = await _context.Services.SingleOrDefaultAsync(x => x.IdService == serviceDTO.IdService);
                if (service != null)
                {
                    var reservationService = new Entities.ReservationService
                    {
                        Reservation = reservation,
                        Service = service,
                        Date = DateTime.Now
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
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null) return null;

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

            var arrival = await _context.Reservations
                .Where(r => r.IdReservation == IdArrival && r.Guest.Person.IdHotel == hotelId)
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

            return arrival;
        }

        public async Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null)
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                };

            var reservation = await _context.Reservations
                .Include(r => r.Deposit)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .SingleOrDefaultAsync(r => r.IdReservation == arrivalDTO.IdReservation && r.Guest.Person.IdHotel == hotelId);

            if (reservation == null)
            {
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Message = "Reservation not found"
                };
            }

            var deposit = reservation.Deposit;

            if (deposit == null && arrivalDTO.IdDepositType != 0)
            {
                var depo = new Deposit
                {
                    Sum = arrivalDTO.DepositSum,
                    IdDepositType = arrivalDTO.IdDepositType
                };
                _context.Deposits.Add(depo);
                reservation.Deposit = depo;
            }
            else if (deposit != null && arrivalDTO.IdDepositType == 0)
            {
                _context.Deposits.Remove(deposit);
                reservation.Deposit = null;
            }
            else if (deposit != null && arrivalDTO.IdDepositType != 0)
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

            _context.ReservationServices.RemoveRange(services.Where(s => !arrivalServiceIds.Contains(s.IdService)));

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
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null)
                return new MethodResultDTO
                {
                    HttpStatusCode = HttpStatusCode.Unauthorized,
                    Message = "Unauthorized"
                };

            var reservation = await _context.Reservations
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.Room)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.IdReservation == IdReservation && r.Guest.Person.IdHotel == hotelId);

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
