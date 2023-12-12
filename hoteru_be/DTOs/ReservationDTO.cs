using System;

namespace hoteru_be.DTOs
{
    public class ReservationDTO
    {
        public int IdReservation { get; set; }
        public string In { get; set; }
        public string Out { get; set; }
        public string BookedBy { get; set; }
        public float Price { get; set; }
        public int Capacity { get; set; }
        public string RoomNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Confirmed { get; set; }
    }
}
