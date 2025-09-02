using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Commands
{
    public class RoomCommandService : IRoomCommandService
    {
        private readonly MyDbContext _context;
        private readonly ILogger<RoomCommandService> _logger;

        public RoomCommandService(MyDbContext context, ILogger<RoomCommandService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MethodResultDTO> DeleteRoom(int hotelId, int idRoom, CancellationToken ct = default)
        {

            var occupiedStatusId = await _context.RoomStatuses
                .AsNoTracking()
                .Where(s => s.Title == "Occupied")
                .Select(s => s.IdRoomStatus)
                .FirstOrDefaultAsync(ct);

            var roomInfo = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.IdRoom == idRoom && r.User.Person.IdHotel == hotelId)
                .Select(r => new { r.IdRoom, r.IdRoomStatus })
                .FirstOrDefaultAsync(ct);

            if (roomInfo is null)
            {
                _logger.LogWarning("DeleteRoom not found: room {RoomId}, hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            if (roomInfo.IdRoomStatus == occupiedStatusId)
            {
                _logger.LogWarning("Attempt to delete occupied room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Conflict("You cannot delete an occupied room");
            }

            try
            {
                var stub = new Room { IdRoom = roomInfo.IdRoom };
                _context.Entry(stub).State = EntityState.Deleted;

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Room {RoomId} deleted in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DeleteRoom conflict for room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Conflict("Cannot delete room due to related data.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DeleteRoom canceled for room {RoomId}", idRoom);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting room {RoomId} in hotel {HotelId}", idRoom, hotelId);
                return MethodResultDTO.Error("Unexpected error while deleting room.");
            }
        }

        public async Task<MethodResultDTO> PostRoom(int hotelId, RoomDTO roomDTO, CancellationToken ct = default)
        {

            var numberExists = await _context.Rooms
                .AnyAsync(r => r.Number == roomDTO.Number && r.User.Person.IdHotel == hotelId, ct);
            if (numberExists)
            {
                return MethodResultDTO.BadRequest(
                    "Room number already exists.",
                    new Dictionary<string, List<string>> { { "Number", new() { "Room number already exists." } } });
            }

            if (!int.TryParse(roomDTO.Status, out var statusId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid status identifier",
                    new Dictionary<string, List<string>> { { "Status", new() { "Status must be a numeric id." } } });
            }
            if (!int.TryParse(roomDTO.Type, out var typeId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid type identifier",
                    new Dictionary<string, List<string>> { { "Type", new() { "Type must be a numeric id." } } });
            }

            var statusExists = await _context.RoomStatuses.AsNoTracking()
                .AnyAsync(s => s.IdRoomStatus == statusId, ct);
            if (!statusExists) return MethodResultDTO.NotFound("Room status not found");

            var typeExists = await _context.RoomTypes.AsNoTracking()
                .AnyAsync(t => t.IdRoomType == typeId, ct);
            if (!typeExists) return MethodResultDTO.NotFound("Room type not found");

            var ownerUser = await _context.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Person.IdHotel == hotelId, ct);
            if (ownerUser is null)
            {
                return MethodResultDTO.Error("No user found for this hotel");
            }
               
            var room = new Room
            {
                Number = roomDTO.Number!,
                Capacity = roomDTO.Capacity ?? 0,
                Price = roomDTO.Price ?? 0m,
                IdRoomType = typeId,
                IdRoomStatus = statusId,
                User = ownerUser
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Room created: room={RoomId}, number={Number}, hotel={HotelId}",
                room.IdRoom, room.Number, hotelId);

            return MethodResultDTO.Created("Created");
        }

        public async Task<MethodResultDTO> UpdateRoom(int hotelId, RoomDTO roomDTO, CancellationToken ct = default)
        {

            var room = await _context.Rooms
                .Include(r => r.User).ThenInclude(u => u.Person)
                .SingleOrDefaultAsync(r => r.IdRoom == roomDTO.IdRoom && r.User.Person.IdHotel == hotelId, ct);

            if (room is null)
            {
                _logger.LogWarning("UpdateRoom: room {RoomId} not found in hotel {HotelId}", roomDTO.IdRoom, hotelId);
                return MethodResultDTO.NotFound("Room not found");
            }

            var numberExists = await _context.Rooms
                .AnyAsync(r => r.Number == roomDTO.Number
                               && r.IdRoom != roomDTO.IdRoom
                               && r.User.Person.IdHotel == hotelId, ct);
            if (numberExists)
            {
                return MethodResultDTO.BadRequest(
                    "Another room with this number already exists",
                    new Dictionary<string, List<string>> { { "Number", new() { "The room number already exists." } } });
            }

            if (!int.TryParse(roomDTO.Status, out var statusId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid status identifier",
                    new Dictionary<string, List<string>> { { "Status", new() { "Status must be a numeric id." } } });
            }
            if (!int.TryParse(roomDTO.Type, out var typeId))
            {
                return MethodResultDTO.Unprocessable(
                    "Invalid type identifier",
                    new Dictionary<string, List<string>> { { "Type", new() { "Type must be a numeric id." } } });
            }

            var statusExists = await _context.RoomStatuses.AsNoTracking()
                .AnyAsync(s => s.IdRoomStatus == statusId, ct);

            if (!statusExists)
            {
                return MethodResultDTO.NotFound("Room status not found");
            }

            var typeExists = await _context.RoomTypes.AsNoTracking()
                .AnyAsync(t => t.IdRoomType == typeId, ct);
            if (!typeExists)
            {
                return MethodResultDTO.NotFound("Room type not found");
            }

            room.Number = roomDTO.Number!;
            if (roomDTO.Capacity is not null)
            {
                room.Capacity = roomDTO.Capacity.Value;
            }

            if (roomDTO.Price is not null)
            {
                room.Price = roomDTO.Price.Value;
            }
            room.IdRoomStatus = statusId;
            room.IdRoomType = typeId;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Room {RoomId} updated in hotel {HotelId}", room.IdRoom, hotelId);
            return MethodResultDTO.Ok("Updated");
        }
    }
}
