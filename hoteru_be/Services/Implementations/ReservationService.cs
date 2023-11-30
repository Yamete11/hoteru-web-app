using hoteru_be.Context;
using hoteru_be.DTOs;
using hoteru_be.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteru_be.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly MyDbContext _context;

        public ReservationService(MyDbContext context)
        {
            _context = context;
        }

        public Task<IEnumerable<ReservationDTO>> GetReservations()
        {
            throw new System.NotImplementedException();
        }
    }
}
