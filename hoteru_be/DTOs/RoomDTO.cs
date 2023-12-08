using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class RoomDTO
    {
        public int IdRoom { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [MaxLength(20, ErrorMessage = "Number can have max 20 symbols")]
        public string? Number { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10, ErrorMessage = "Capacity must be a number between 1 and 10")]
        public int? Capacity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be a number between 0.01 and 1,000,000")]
        public float? Price { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string? Type { get; set; }

    }
}
