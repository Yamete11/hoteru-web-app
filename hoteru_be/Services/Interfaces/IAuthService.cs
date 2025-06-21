using hoteru_be.DTOs;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string ErrorMessage)> AuthenticateAsync(LoginDTO loginDTO);
    }
}
