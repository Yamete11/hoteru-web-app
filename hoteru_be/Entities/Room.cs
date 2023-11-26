using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Room number must be at most 50 characters.")]
        public string Number { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10.")]
        public int Capacity { get; set; }

        [Required]
        [Range(1.0, 10000.0, ErrorMessage = "Price must be between 1 and 10000")]
        [DataType(DataType.Currency)]
        public float Price { get; set; }

        public int IdRoomStatus { get; set; }

        [Required]
        [ForeignKey(nameof(IdRoomStatus))]
        public RoomStatus RoomStatus { get; set; }

        public int IdRoomType { get; set; }

        [Required]
        [ForeignKey(nameof(IdRoomType))]
        public RoomType RoomType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
