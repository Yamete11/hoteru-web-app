using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }

        [Required(ErrorMessage = "The room number is required")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Room number must be at most 3 characters")]
        public string Number { get; set; }

        [Required(ErrorMessage = "The room capacity is required")]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "The room price is required")]
        [Range(1.0, 10.0, ErrorMessage = "Price must be between 1 and 10")]
        public float Price { get; set; }

        [Required(ErrorMessage = "The room status is required")]
        public int IdRoomStatus { get; set; }

        
        [ForeignKey(nameof(IdRoomStatus))]
        public RoomStatus RoomStatus { get; set; }


        [Required(ErrorMessage = "The room type is required")]
        public int IdRoomType { get; set; }

        
        [ForeignKey(nameof(IdRoomType))]
        public RoomType RoomType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
