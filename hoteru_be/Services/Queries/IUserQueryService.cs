using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IUserQueryService
    {
        Task<MethodResultDTO<UserDTO>> GetUser(int hotelId, string userName, CancellationToken ct = default);
        Task<MethodResultDTO<List<ListUserDTO>>> GetUsers(int hotelId, CancellationToken ct = default);
        Task<MethodResultDTO<FullUserDTO>> GetFullUser(int hotelId, int idUser, CancellationToken ct = default);
    }
}
