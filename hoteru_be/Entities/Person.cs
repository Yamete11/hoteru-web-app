using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Person
    {
        [Key]
        public int IdPerson { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
