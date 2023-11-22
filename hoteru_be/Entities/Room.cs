using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }

        public string Number { get; set; }

        public int Capacity { get; set; }
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
