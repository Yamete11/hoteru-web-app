using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Commands
{
    public interface IUserCommandService
    {
        Task<MethodResultDTO> PostUser(int hotelId, NewUserDTO newUserDTO, CancellationToken ct = default);
        Task<MethodResultDTO> UpdateUser(int hotelId,
    string currentRole,
    int currentPersonId,
    UpdateUserDTO dto,
    CancellationToken ct = default);
        Task<MethodResultDTO> DeleteUser(int hotelId, int idPerson, CancellationToken ct = default);
    }
}
