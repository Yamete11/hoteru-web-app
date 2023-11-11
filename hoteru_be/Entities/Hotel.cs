using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Hotel
    {
        [Key]
        public int IdHotel { get; set; }
        public string Title { get; set; }

        public int IdAddress { get; set; }

        [Required]
        [ForeignKey(nameof(IdAddress))]
        public Address Address { get; set; }

    }
}
