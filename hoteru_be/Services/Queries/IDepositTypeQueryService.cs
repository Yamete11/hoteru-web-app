using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IDepositTypeQueryService
    {
        Task<MethodResultDTO<List<TypeDTO>>> GetDepositTypes(CancellationToken ct);
    }
}
