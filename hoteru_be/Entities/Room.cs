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
        public string Number { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public float Price { get; set; }

        [Required]
        public int IdRoomStatus { get; set; }

        
        [ForeignKey(nameof(IdRoomStatus))]
        public RoomStatus RoomStatus { get; set; }


        [Required]
        public int IdRoomType { get; set; }

        
        [ForeignKey(nameof(IdRoomType))]
        public RoomType RoomType { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
