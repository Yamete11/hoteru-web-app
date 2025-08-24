using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public interface IUserService
    {
        public Task<MethodResultDTO<UserDTO>> GetUser(string userName, CancellationToken ct);
        public Task<MethodResultDTO<List<ListUserDTO>>> GetUsers(CancellationToken ct);
        public Task<MethodResultDTO<FullUserDTO>> GetFullUser(int idUser, CancellationToken ct);

        public Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO, CancellationToken ct);

        public Task<MethodResultDTO> UpdateUser(UpdateUserDTO updateUserDTO, CancellationToken ct);

        public Task<MethodResultDTO> DeleteUser(int IdPerson, CancellationToken ct);
    }
}
