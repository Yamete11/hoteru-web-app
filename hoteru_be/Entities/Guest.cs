using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Guest
    {
        [Key]
        public int IdGuest { get; set; }
    }
}
