using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hoteru_be.Services.Commands
{
    public class UserCommandService : IUserCommandService
    {
        private readonly MyDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserCommandService> _logger;

        public UserCommandService(MyDbContext context, IPasswordHasher<User> passwordHasher, ILogger<UserCommandService> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<MethodResultDTO> PostUser(int hotelId, NewUserDTO dto, CancellationToken ct = default)
        {

            var login = dto.LoginName?.Trim();
            var email = dto.Email?.Trim();

            if (await _context.Users.AsNoTracking().AnyAsync(u => u.LoginName == login, ct))
            {
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "LoginName", new() { "Another user with this login already exists." } } });
            }

            if (await _context.Persons.AsNoTracking().AnyAsync(p => p.Email == email && p.IdHotel == hotelId, ct))
            {
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "Email", new() { "Another person with this email already exists." } } });
            }

            var userTypeExists = await _context.UserTypes.AsNoTracking()
                .AnyAsync(t => t.IdUserType == dto.IdUserType, ct);
            if (!userTypeExists)
            {
                return MethodResultDTO.NotFound("User type not found");
            }

            var person = new Person
            {
                Name = dto.Name.Trim(),
                Surname = dto.Surname.Trim(),
                Email = email!,
                IdHotel = hotelId
            };

            var user = new User
            {
                LoginName = login!,
                IdUserType = dto.IdUserType,
                Person = person
            };
            user.Password = _passwordHasher.HashPassword(user, dto.Password);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);
                _logger?.LogInformation("User created: personId={PersonId}, hotel={HotelId}", user.IdPerson, hotelId);
                return MethodResultDTO.Created("Created");
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating user {Login}", login);
                return MethodResultDTO.Error("An error occurred while creating user.");
            }
        }

        public async Task<MethodResultDTO> UpdateUser(
    int hotelId,
    string currentRole,
    int currentPersonId,
    UpdateUserDTO dto,
    CancellationToken ct = default)
        {
            var login = (dto.LoginName ?? string.Empty).Trim();
            var email = (dto.Email ?? string.Empty).Trim();
            var name = (dto.Name ?? string.Empty).Trim();
            var surname = (dto.Surname ?? string.Empty).Trim();

            var target = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.UserType)
                .SingleOrDefaultAsync(u => u.IdPerson == dto.IdPerson && u.Person.IdHotel == hotelId, ct);
            if (target is null)
                return MethodResultDTO.NotFound("User not found");

            var targetRole = target.UserType?.Title ?? string.Empty;

            var requestedRole = await _context.UserTypes
                .AsNoTracking()
                .Where(t => t.IdUserType == dto.IdUserType)
                .Select(t => t.Title)
                .SingleOrDefaultAsync(ct);
            if (requestedRole is null)
                return MethodResultDTO.NotFound("User type not found");

            bool isSelf = currentPersonId == dto.IdPerson;
            bool isSuperadmin = currentRole.Equals("Superadmin", StringComparison.OrdinalIgnoreCase);
            bool isAdmin = currentRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            bool isEmployee = currentRole.Equals("Employee", StringComparison.OrdinalIgnoreCase);
            bool roleChanged = dto.IdUserType != target.IdUserType;

            if (isEmployee)
            {
                if (!isSelf) return MethodResultDTO.Forbidden("Employees can update only their own profile.");
                if (roleChanged) { dto.IdUserType = target.IdUserType; roleChanged = false; }
            }
            else if (isAdmin)
            {
                if (!(targetRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                      targetRole.Equals("Employee", StringComparison.OrdinalIgnoreCase)))
                    return MethodResultDTO.Forbidden("Admins can update only Admin/Employee users.");

                if (roleChanged)
                {
                    if (!requestedRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) &&
                        !requestedRole.Equals("Employee", StringComparison.OrdinalIgnoreCase))
                        return MethodResultDTO.Forbidden("Admins cannot assign this role.");
                }
            }
            else if (isSuperadmin)
            {
                if (roleChanged && requestedRole.Equals("Superadmin", StringComparison.OrdinalIgnoreCase) &&
                    !targetRole.Equals("Superadmin", StringComparison.OrdinalIgnoreCase))
                    return MethodResultDTO.Forbidden("Cannot assign Superadmin role to another user.");

                if (isSelf && roleChanged)
                    return MethodResultDTO.Forbidden("Superadmin cannot change their own role.");
            }
            else
            {
                return MethodResultDTO.Forbidden("Unknown role.");
            }

            var loginExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.LoginName == login && u.IdPerson != dto.IdPerson, ct);
            if (loginExists)
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "LoginName", new() { "Another user with this login already exists." } } });

            var emailExists = await _context.Persons
                .AsNoTracking()
                .AnyAsync(p => p.Email == email && p.IdHotel == hotelId && p.IdPerson != dto.IdPerson, ct);
            if (emailExists)
                return MethodResultDTO.BadRequest("Validation failed",
                    new() { { "Email", new() { "Another person with this email already exists." } } });

            try
            {
                target.LoginName = login;
                target.Person.Name = name;
                target.Person.Surname = surname;
                target.Person.Email = email;
                if (roleChanged)
                    target.IdUserType = dto.IdUserType;

                await _context.SaveChangesAsync(ct);
                _logger?.LogInformation("User {PersonId} updated in hotel {HotelId}", target.IdPerson, hotelId);
                return MethodResultDTO.Ok("Updated");
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error updating user {PersonId}", dto.IdPerson);
                return MethodResultDTO.Error("An error occurred while updating user.");
            }
        }


        public async Task<MethodResultDTO> DeleteUser(int hotelId, int idPerson, CancellationToken ct = default)
        {
            var user = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.Reservations)
                .SingleOrDefaultAsync(u => u.IdPerson == idPerson && u.Person.IdHotel == hotelId, ct);

            if (user is null)
            {
                _logger?.LogWarning("User {PersonId} not found in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.NotFound("User not found");
            }

            var superAdminId = await _context.Users
                .AsNoTracking()
                .Where(u => u.IdUserType == 1)
                .Select(u => u.IdPerson)
                .FirstOrDefaultAsync(ct);

            if (superAdminId == 0)
            {
                _logger?.LogError("Super admin user not found");
                return MethodResultDTO.Error("Super admin user not found");
            }

            try
            {
                foreach (var r in user.Reservations)
                {
                    r.IdUser = superAdminId;
                }

                _context.Users.Remove(user);
                _context.Persons.Remove(user.Person);

                await _context.SaveChangesAsync(ct);

                _logger?.LogInformation("User {PersonId} deleted in hotel {HotelId}", idPerson, hotelId);
                return MethodResultDTO.Ok("Deleted");
            }
            catch (OperationCanceledException)
            {
                _logger?.LogWarning("DeleteUser canceled for person {PersonId}", idPerson);
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error deleting user {PersonId}", idPerson);
                return MethodResultDTO.Error("Unexpected error while deleting user.");
            }
        }
    }
}
