using System;
using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class PostReservationDTO
    {
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public float Price { get; set; }
        public int Capacity { get; set; }
        public int IdRoom { get; set; }
        public bool Confirmed { get; set; }
        public float Sum { get; set; }
        public int IdDepositType { get; set; }
        public List<ServiceD> Services { get; set; }
        public int IdPerson { get; set; }
        public int IdUser { get; set; }
    }
}
