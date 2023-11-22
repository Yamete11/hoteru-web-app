using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class User
    {
        [Key]
        public int IdPerson { get; set; }

        [Required]
        [ForeignKey(nameof(IdPerson))]
        public Person Person { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }

        public int IdUserType { get; set; }

        [Required]
        [ForeignKey(nameof(IdUserType))]
        public UserType UserType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
