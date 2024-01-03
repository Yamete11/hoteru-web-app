using hoteru_be.DTOs;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IDepositService
    {
        public Task<DepositDTO> GetDeposit(int IdDeposit);
    }
}
