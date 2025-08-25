// Services/Commands/ReservationCommandService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Commands
{
    public class ReservationCommandService : IReservationCommandService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ReservationCommandService> _logger;

        public ReservationCommandService(MyDbContext context, ILogger<ReservationCommandService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO> PostReservation(int hotelId, PostReservationDTO reservationDTO, CancellationToken ct = default)
        {
            var room = await _context.Rooms
                .Include(r => r.User).ThenInclude(u => u.Person)
                .SingleOrDefaultAsync(r => r.IdRoom == reservationDTO.IdRoom && r.User.Person.IdHotel == hotelId, ct);

            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found for hotel {HotelId}", reservationDTO.IdRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            if (room.IdRoomStatus != 3)
            {
                return MethodResultDTO.BadRequest("Room is occupied");
            }

            Deposit? deposit = null;
            if (reservationDTO.IdDepositType != 0)
            {
                deposit = new Deposit { Sum = reservationDTO.Sum, IdDepositType = reservationDTO.IdDepositType };
                _context.Deposits.Add(deposit);
            }

            var guest = await _context.Guests
                .Include(g => g.Person)
                .SingleOrDefaultAsync(g => g.IdPerson == reservationDTO.IdPerson && g.Person.IdHotel == hotelId, ct);

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

            foreach (var s in reservationDTO.Services)
            {
                var service = await _context.Services.SingleOrDefaultAsync(x => x.IdService == s.IdService, ct);
                if (service != null)
                {
                    _context.ReservationServices.Add(new Entities.ReservationService
                    {
                        Reservation = reservation,
                        Service = service,
                        Date = DateTime.Now
                    });
                }
            }

            room.IdRoomStatus = 2;
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Reservation created: reservation={ReservationId}, room={RoomId}, hotel={HotelId}", reservation.IdReservation, room.IdRoom, hotelId);
            return MethodResultDTO.Created("Created");
        }

        public async Task<MethodResultDTO> UpdateReservation(int hotelId, ArrivalDTO arrivalDTO, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Deposit)
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .SingleOrDefaultAsync(r => r.IdReservation == arrivalDTO.IdReservation && r.Guest.Person.IdHotel == hotelId, ct);

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
                    var depo = new Deposit { Sum = arrivalDTO.DepositSum, IdDepositType = arrivalDTO.IdDepositType };
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

                var current = await _context.ReservationServices
                    .Where(r => r.IdReservation == arrivalDTO.IdReservation)
                    .ToListAsync(ct);

                var incomingIds = arrivalDTO.Services.Select(s => s.IdService).ToHashSet();

                _context.ReservationServices.RemoveRange(current.Where(s => !incomingIds.Contains(s.IdService)));

                foreach (var serviceId in incomingIds)
                {
                    if (!current.Any(s => s.IdService == serviceId))
                    {
                        _context.ReservationServices.Add(new Entities.ReservationService
                        {
                            IdReservation = arrivalDTO.IdReservation,
                            IdService = serviceId,
                            Date = DateTime.Now
                        });
                    }
                }

                await _context.SaveChangesAsync(ct);
                _logger.LogInformation("Reservation {ReservationId} updated successfully", arrivalDTO.IdReservation);
                return MethodResultDTO.Ok("Updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating reservation {ReservationId}", arrivalDTO.IdReservation);
                return MethodResultDTO.Error("An error occurred while updating reservation");
            }
        }

        public async Task<MethodResultDTO> DeleteSpecificReservation(int hotelId, int idReservation, CancellationToken ct = default)
        {


            var reservation = await _context.Reservations
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .SingleOrDefaultAsync(x => x.IdReservation == idReservation && x.Guest.Person.IdHotel == hotelId, ct);

            if (reservation == null)
            {
                return MethodResultDTO.NotFound("Reservation not found");
            }
               
            var reservationServices = await _context.ReservationServices
                .Where(x => x.IdReservation == idReservation)
                .ToListAsync(ct);

            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.IdRoom == reservation.IdRoom, ct);
            if (room != null) room.IdRoomStatus = 1;

            _context.ReservationServices.RemoveRange(reservationServices);
            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync(ct);
            return MethodResultDTO.Ok("Reservation deleted successfully");
        }

        public async Task<MethodResultDTO> ConfirmReservation(int hotelId, int reservationId, CancellationToken ct = default)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Guest).ThenInclude(g => g.Person)
                .Include(r => r.Room)
                .Include(r => r.User)
                .SingleOrDefaultAsync(r => r.IdReservation == reservationId && r.Guest.Person.IdHotel == hotelId, ct);

            if (reservation == null)
                return MethodResultDTO.NotFound("Reservation not found");

            try
            {
                var statuses = await _context.RoomStatuses
                    .AsNoTracking()
                    .ToDictionaryAsync(s => s.Title, s => s.IdRoomStatus, ct);

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

                await _context.SaveChangesAsync(ct);
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
