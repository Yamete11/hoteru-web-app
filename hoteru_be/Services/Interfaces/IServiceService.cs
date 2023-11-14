using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IServiceService
    {
        public Task<IEnumerable<ServiceDTO>> GetServices();
        public Task<MethodResultDTO> DeleteService(int IdService);
    }
}
