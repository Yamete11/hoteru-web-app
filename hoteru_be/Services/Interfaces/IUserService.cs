using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO> GetUser(string userName);
        public Task<List<ListUserDTO>> GetUsers();
        public Task<FullUserDTO> GetFullUser(int idUser);

        public Task<MethodResultDTO> PostUser(NewUserDTO newUserDTO);

        public Task<MethodResultDTO> UpdateUser(UpdateUserDTO updateUserDTO);

        public Task<MethodResultDTO> DeleteUser(int IdPerson);
    }
}
