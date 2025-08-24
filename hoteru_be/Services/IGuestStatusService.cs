using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services
{
    public interface IGuestStatusService
    {
        Task<MethodResultDTO<List<StatusDTO>>> GetGuestStatuses(CancellationToken ct);
    }
}
