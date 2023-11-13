using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Address
    {
        [Key]
        public int IdAddress { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string Postcode { get; set; }

        public Hotel Hotel { get; set; }

    }
}
