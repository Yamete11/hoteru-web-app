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
        [MaxLength(16)]
        public string Number { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdRoomStatus { get; set; }


        [ForeignKey(nameof(IdRoomStatus))]
        public RoomStatus RoomStatus { get; set; }


        [Required]
        [Range(1, int.MaxValue)]
        public int IdRoomType { get; set; }


        [ForeignKey(nameof(IdRoomType))]
        public RoomType RoomType { get; set; }

        public int IdUser { get; set; }
        [Required]
        [ForeignKey(nameof(IdUser))]
        public User User { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
