using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Guest
    {
        [Key]
        public int IdPerson { get; set; }

        [Required]
        [ForeignKey(nameof(IdPerson))]
        public Person Person { get; set; }

        public string Passport { get; set; }
        public string TelNumber { get; set; }

        public int IdGuestStatus { get; set; }

        [Required]
        [ForeignKey(nameof(IdGuestStatus))]
        public GuestStatus GuestStatus { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
