using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Service
    {
        [Key]
        public int IdService { get; set; }
        public string Title { get; set; }
        public float Sum { get; set; }
        public string Description { get; set; }

        public int IdUser { get; set; }
        [Required]
        [ForeignKey(nameof(IdUser))]
        public User User { get; set; }

        public virtual ICollection<ReservationService> ReservationServices { get; set; }

    }
}
