using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class GuestReservation
    {
        [Key]
        public int IdGuestReservation { get; set; }

       
        public int IdGuest { get; set; }

        [Required]
        [ForeignKey(nameof(IdGuest))]
        public Guest Guest { get; set; }

        public int IdReservation { get; set; }

        [Required]
        [ForeignKey(nameof(IdReservation))]
        public Reservation Reservation { get; set; }

    }
}
