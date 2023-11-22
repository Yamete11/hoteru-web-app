using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class ReservationService
    {
        [Key]
        public int IdReservationService { get; set; }
        public DateTime Date { get; set; }

        public int IdService { get; set; }

        [Required]
        [ForeignKey(nameof(IdService))]
        public Service Service { get; set; }

        public int IdReservation { get; set; }

        [Required]
        [ForeignKey(nameof(IdReservation))]
        public Reservation Reservation { get; set; }
    }
}
