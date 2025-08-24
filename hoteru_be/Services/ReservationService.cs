using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ReservationService : IReservationService
    {
        private readonly MyDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            MyDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReservationService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private int? GetHotelIdFromToken()
        {
            var hotelIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("hotelId")?.Value;
            return int.TryParse(hotelIdClaim, out int hotelId) ? hotelId : null;
        }

        public async Task<PaginatedResultDTO<ReservationDTO>> GetReservations(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetReservations request");
                return new PaginatedResultDTO<ReservationDTO>
                {
                    List = new List<ReservationDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill == null && r.Confirmed && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name":
                        query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term));
                        break;
                    case "roomnumber":
                        query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term));
                        break;
                    case "bookedby":
                        query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term));
                        break;
                }
            }

            var total = await query.CountAsync(ct);

            var list = await query
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
                .ToListAsync(ct);

            _logger.LogInformation("Fetched reservations: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<PaginatedResultDTO<ReservationDTO>> GetHistory(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {

            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetHistory request");
                return new PaginatedResultDTO<ReservationDTO>
                {
                    List = new List<ReservationDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill != null && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name":
                        query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term));
                        break;
                    case "roomnumber":
                        query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term));
                        break;
                    case "bookedby":
                        query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term));
                        break;
                }
            }

            var total = await query.CountAsync(ct);

            var list = await query
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
                .ToListAsync(ct);

            _logger.LogInformation("Fetched history: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<PaginatedResultDTO<ReservationDTO>> GetArrivals(int page, int limit, string searchQuery = "", string searchField = "", CancellationToken ct = default)
        {

            page = page < 1 ? 1 : page;
            limit = limit < 1 ? 10 : limit;

            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetArrivals request");
                return new PaginatedResultDTO<ReservationDTO>
                {
                    List = new List<ReservationDTO>(),
                    TotalCount = 0,
                    Page = page,
                    Limit = limit
                };
            }

            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => !r.Confirmed && r.Guest.Person.IdHotel == hotelId);

            if (!string.IsNullOrWhiteSpace(searchQuery) && !string.IsNullOrWhiteSpace(searchField))
            {
                var term = $"{searchQuery.Trim().ToLower()}%";
                switch (searchField.Trim().ToLower())
                {
                    case "name":
                        query = query.Where(r => EF.Functions.Like(r.Guest.Person.Name.ToLower(), term));
                        break;
                    case "roomnumber":
                        query = query.Where(r => EF.Functions.Like(r.Room.Number.ToLower(), term));
                        break;
                    case "bookedby":
                        query = query.Where(r => EF.Functions.Like(r.User.LoginName.ToLower(), term));
                        break;
                }
            }

            var total = await query.CountAsync(ct);

            var list = await query
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
                .ToListAsync(ct);

            _logger.LogInformation("Fetched arrivals: hotel={HotelId}, page={Page}, limit={Limit}, total={Total}",
                hotelId, page, limit, total);

            return new PaginatedResultDTO<ReservationDTO>
            {
                List = list,
                TotalCount = total,
                Page = page,
                Limit = limit
            };
        }


        public async Task<MethodResultDTO<FullReservationDTO>> GetSpecificHistory(int idReservation, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized GetSpecificHistory for reservation {ReservationId}", idReservation);
                return MethodResultDTO<FullReservationDTO>.Unauthorized("Unauthorized");
            }

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.Bill != null && r.IdReservation == idReservation && r.Guest.Person.IdHotel == hotelId)
                .Include(r => r.Bill)
                .Include(r => r.Room).ThenInclude(rt => rt.RoomType)
                .Include(r => r.Deposit).ThenInclude(d => d.DepositType)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.User).ThenInclude(u => u.Person)
                .FirstOrDefaultAsync();

            if (reservation is null)
            {
                _logger.LogWarning("History not found: reservation {ReservationId}, hotel {HotelId}", idReservation, hotelId);
                return MethodResultDTO<FullReservationDTO>.NotFound("Reservation history not found");
            }

            var services = await _context.ReservationServices
                .AsNoTracking()
                .Where(rs => rs.IdReservation == idReservation)
                .Include(rs => rs.Service)
                .Select(rs => new ServiceHistoryDTO
                {
                    IdService = rs.IdService,
                    Title = rs.Service.Title,
                    Sum = rs.Service.Sum,
                    Date = rs.Date
                })
                .ToListAsync();

            var dto = new FullReservationDTO
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
                DepositType = reservation.Deposit?.DepositType?.Title ?? string.Empty,
                BillSum = reservation.Bill!.Sum,
                Created = reservation.Bill.Created.ToString("yyyy-MM-dd"),
                Services = services
            };

            _logger.LogInformation("Fetched reservation history {ReservationId} for hotel {HotelId} with {ServicesCount} services",
                idReservation, hotelId, services.Count);

            return MethodResultDTO<FullReservationDTO>.Ok(dto, "Fetched");
        }

        public async Task<MethodResultDTO> DeleteSpecificReservation(int idReservation, CancellationToken ct)
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


        public async Task<MethodResultDTO> PostReservation(PostReservationDTO reservationDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized PostReservation attempt");
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var room = await _context.Rooms
                .Include(r => r.User).ThenInclude(u => u.Person)
                .SingleOrDefaultAsync(r => r.IdRoom == reservationDTO.IdRoom && r.User.Person.IdHotel == hotelId);

            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found for hotel {HotelId}", reservationDTO.IdRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            if (room.IdRoomStatus != 3)
            {
                return MethodResultDTO.BadRequest("Room is occupied");
               
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

            if (guest is null)
            {
                _logger.LogWarning("Guest {PersonId} not found for hotel {HotelId}", reservationDTO.IdPerson, hotelId);
                return MethodResultDTO.NotFound("Guest not found");
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

            _logger.LogInformation("Reservation created: reservation={ReservationId}, room={RoomId}, hotel={HotelId}",
            reservation.IdReservation, room.IdRoom, hotelId);

            return MethodResultDTO.Created("Created");
        }

        public async Task<MethodResultDTO<ArrivalDTO>> GetSpecificArrival(int idArrival, CancellationToken cancellationToken)
        {
            try
            {
                var hotelId = GetHotelIdFromToken();
                if (hotelId is null)
                {
                    _logger.LogWarning("Unauthorized GetSpecificArrival for reservation {ReservationId}", idArrival);
                    return MethodResultDTO<ArrivalDTO>.Unauthorized("Unauthorized");
                }

                var services = await _context.ReservationServices
                    .AsNoTracking()
                    .Where(rs => rs.IdReservation == idArrival)
                    .Include(rs => rs.Service)
                    .Select(rs => new ServiceHistoryDTO
                    {
                        IdService = rs.IdService,
                        Title = rs.Service.Title,
                        Sum = rs.Service.Sum,
                        Date = rs.Date
                    })
                    .ToListAsync(cancellationToken);

                var arrival = await _context.Reservations
                    .AsNoTracking()
                    .Where(r => r.IdReservation == idArrival && r.Guest.Person.IdHotel == hotelId)
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
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (arrival is null)
                {
                    _logger.LogWarning("Arrival not found: reservation {ReservationId}, hotel {HotelId}", idArrival, hotelId);
                    return MethodResultDTO<ArrivalDTO>.NotFound("Arrival not found");
                }

                _logger.LogInformation("Fetched arrival {ReservationId} for hotel {HotelId} with {ServicesCount} services",
                    idArrival, hotelId, services.Count);

                return MethodResultDTO<ArrivalDTO>.Ok(arrival, "Fetched");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetSpecificArrival canceled for reservation {ReservationId}", idArrival);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching arrival {ReservationId}", idArrival);
                return MethodResultDTO<ArrivalDTO>.Error("Failed to fetch arrival");
            }
        }


        public async Task<MethodResultDTO> UpdateReservation(ArrivalDTO arrivalDTO, CancellationToken ct)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId == null)
            {
                _logger.LogWarning("Unauthorized attempt to update reservation {ReservationId}", arrivalDTO.IdReservation);
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var reservation = await _context.Reservations
                .Include(r => r.Deposit)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .SingleOrDefaultAsync(r => r.IdReservation == arrivalDTO.IdReservation && r.Guest.Person.IdHotel == hotelId);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found for hotel {HotelId}", arrivalDTO.IdReservation, hotelId);
                return MethodResultDTO.NotFound("Reservation not found");
            }

            try
            {
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

                var services = await _context.ReservationServices
                    .Where(r => r.IdReservation == arrivalDTO.IdReservation)
                    .ToListAsync();

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
                _logger.LogInformation("Reservation {ReservationId} updated successfully", arrivalDTO.IdReservation);
                return MethodResultDTO.Ok("Updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating reservation {ReservationId}", arrivalDTO.IdReservation);
                return MethodResultDTO.Error("An error occurred while updating reservation");
            }
        }


        public async Task<MethodResultDTO> ConfirmReservation(int reservationId, CancellationToken cancellationToken)
        {
            var hotelId = GetHotelIdFromToken();
            if (hotelId is null)
            {
                _logger.LogWarning("Unauthorized attempt to confirm reservation {ReservationId}", reservationId);
                return MethodResultDTO.Unauthorized("Unauthorized");
            }

            var reservation = await _context.Reservations
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.Room)
                .Include(r => r.User)
                .SingleOrDefaultAsync(
                    r => r.IdReservation == reservationId && r.Guest.Person.IdHotel == hotelId,
                    cancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning("Reservation {ReservationId} not found for hotel {HotelId}", reservationId, hotelId);
                return MethodResultDTO.NotFound("Reservation not found");
            }

            try
            {
                var statuses = await _context.RoomStatuses
                    .AsNoTracking()
                    .ToDictionaryAsync(s => s.Title, s => s.IdRoomStatus, cancellationToken);

                var room = reservation.Room;

                if (!reservation.Confirmed)
                {
                    reservation.Confirmed = true;
                    room.IdRoomStatus = statuses["Occupied"];
                    _logger.LogInformation("Reservation {ReservationId} confirmed for hotel {HotelId}", reservationId, hotelId);
                }
                else
                {
                    reservation.Bill = new Bill
                    {
                        Created = DateTime.UtcNow,
                        Sum = reservation.Price,
                        InDate = reservation.In,
                        OutDate = reservation.Out,
                        GuestName = reservation.Guest?.Person?.Name ?? "Unknown",
                        GuestSurname = reservation.Guest?.Person?.Surname ?? "Unknown",
                        RoomNumber = room.Number,
                        BookedBy = reservation.User?.LoginName ?? "Unknown"
                    };

                    room.IdRoomStatus = statuses["Out of service"];
                    _logger.LogInformation("Reservation {ReservationId} closed and bill created", reservationId);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return MethodResultDTO.Ok("Confirmed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while confirming reservation {ReservationId}", reservationId);
                return MethodResultDTO.Error("An error occurred while confirming reservation");
            }
        }
    }
}
