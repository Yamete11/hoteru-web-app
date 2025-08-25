using hoteru_be.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace hoteru_be.Services.Queries
{
    public interface IDepositQueryService
    {
        Task<MethodResultDTO<DepositDTO>> GetDeposit(int idDeposit, CancellationToken ct);
    }
}
