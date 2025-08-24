using hoteru_be.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services
{
    public interface IDepositService
    {
        Task<MethodResultDTO<DepositDTO>> GetDeposit(int idDeposit, CancellationToken ct);
    }
}
