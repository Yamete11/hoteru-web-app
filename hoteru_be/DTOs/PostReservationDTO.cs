using System;

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
        public GuestD guest { get; set; }
    }
}
