using System;
using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class ArrivalDTO
    {
        public int IdReservation { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public int Capacity { get; set; }
        public int IdRoom { get; set; }
        public int IdRoomType { get; set; }
        public int IdGuest { get; set; }
        public int IdDepositType { get; set; }
        public float DepositSum { get; set; }
        public float Price { get; set; }
        public bool Confirmed { get; set; }

        public List<ServiceHistoryDTO> Services { get; set; }
    }
}
