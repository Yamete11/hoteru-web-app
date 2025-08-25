using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IUserCommandService
    {
        Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateUser(UpdateUserDTO updateUserDTO, CancellationToken ct = default);
        Task<MethodResultDTO> DeleteUser(int idPerson, CancellationToken ct = default);
    }
}
