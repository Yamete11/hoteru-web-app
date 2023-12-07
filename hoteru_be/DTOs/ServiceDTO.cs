using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class ServiceDTO
    {
        public int IdService { get; set; }


        [Required(ErrorMessage = "Title is required")]
        [MaxLength(20, ErrorMessage = "Title can have max 20 symbols")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public float? Sum { get; set; }

        [MaxLength(50, ErrorMessage = "Description can have max 50 symbols")]
        public string Description { get; set; }
    }
}
