using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Reservation
    {
        [Key]
        public int IdReservation { get; set; }
        public int Capacity { get; set; }
        public float Price { get; set; }
        public DateTime In { get; set; }
        public DateTime Out { get; set; }


        public int IdRoom { get; set; }
        [Required]
        [ForeignKey(nameof(IdRoom))]
        public Room Room { get; set; }



        public int IdDeposit { get; set; }
        [Required]
        [ForeignKey(nameof(IdDeposit))]
        public Deposit Deposit { get; set; }



        public int IdUser { get; set; }
        [Required]
        [ForeignKey(nameof(IdUser))]
        public User User { get; set; }

        public int? IdBill { get; set; }
        [ForeignKey(nameof(IdBill))]
        public Bill Bill { get; set; }

        public virtual ICollection<GuestReservation> GuestReservations { get; set; }
        public virtual ICollection<ReservationService> ReservationServices { get; set; }

    }
}
