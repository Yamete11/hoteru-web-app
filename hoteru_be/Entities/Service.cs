using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Service
    {
        [Key]
        public int IdService { get; set; }
        public string Title { get; set; }
        public float Sum { get; set; }
        public string Description { get; set; }

    }
}
