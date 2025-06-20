using System;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Bill
    {
        [Key]
        public int IdBill { get; set; }
        public DateTime Created { get; set; }
        public float Sum { get; set; }


        [Required]
        public DateTime InDate { get; set; }

        [Required]
        public DateTime OutDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string GuestName { get; set; }

        [Required]
        [MaxLength(20)]
        public string GuestSurname { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string BookedBy { get; set; }

        public Reservation Reservation { get; set; }
    }
}
