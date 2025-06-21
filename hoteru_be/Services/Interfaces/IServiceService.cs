using hoteru_be.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hoteru_be.Services.Interfaces
{
    public interface IServiceService
    {
        public Task<PaginatedResultDTO<ServiceDTO>> GetServices(int page, int limit, string searchField, string searchQuery);
        public Task<ServiceDTO> GetSpecificService(int idService);
        public Task<MethodResultDTO> DeleteService(int idService);

        public Task<MethodResultDTO> PostService(ServiceDTO serviceDTO);

        public Task<MethodResultDTO> UpdateService(ServiceDTO serviceDTO);
    }
}
