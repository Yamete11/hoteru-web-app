using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class GuestStatus
    {
        [Key]
        public int IdGuestStatus { get; set; }
        public string Title { get; set; }
        public ICollection<Guest> Guests { get; set; }
    }
}
