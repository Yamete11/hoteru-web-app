using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IUserQueryService
    {
        Task<MethodResultDTO<UserDTO>> GetUser(string userName, CancellationToken ct = default);
        Task<MethodResultDTO<List<ListUserDTO>>> GetUsers(CancellationToken ct = default);
        Task<MethodResultDTO<FullUserDTO>> GetFullUser(int idUser, CancellationToken ct = default);
    }
}
