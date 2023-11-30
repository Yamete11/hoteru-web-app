using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class RoomDTO
    {
        public int IdRoom { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [MaxLength(10, ErrorMessage = "Number can have max 10 symbols")]
        public string? Number { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number")]
        public int? Capacity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public float? Price { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

    }
}
