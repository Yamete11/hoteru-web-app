using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using hoteru_be.DTOs;

namespace hoteru_be.Services.Queries
{
    public interface IRoomTypeQueryService
    {
        Task<MethodResultDTO<List<TypeDTO>>> GetRoomTypes(CancellationToken ct);
    }
}
