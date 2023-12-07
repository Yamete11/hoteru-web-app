using System;
using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class FullReservationDTO
    {
        public int IdReservation { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string BookedBy { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float DepositSum { get; set; }
        public string DepositType { get; set; }
        public float BillSum { get; set; }
        public string Created { get; set; }

        public List<ServiceHistoryDTO> Services { get; set; }

    }
}
