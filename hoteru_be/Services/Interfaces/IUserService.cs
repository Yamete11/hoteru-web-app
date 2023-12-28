using hoteru_be.DTOs;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO> GetUser(string userName);
        public Task<FullUserDTO> GetFullUser(int idUser);

        public Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO);
    }
}
