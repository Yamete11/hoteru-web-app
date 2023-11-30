using System;

namespace hoteru_be.DTOs
{
    public class ReservationDTO
    {
        public int IdReservation { get; set; }
        public int Capacity { get; set; }
        public float Price { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }
        public string BookedBy { get; set; }
        public string RoomNumber { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
