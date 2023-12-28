using System;
using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class ArrivalDTO
    {
        public int IdReservation { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public int Capacity { get; set; }
        public int IdRoom { get; set; }
        public int IdGuest { get; set; }
        public int IdDeposit { get; set; }
        public float Price { get; set; }

        public List<ServiceHistoryDTO> Services { get; set; }
    }
}
