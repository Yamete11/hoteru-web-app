using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class RoomStatus
    {
        [Key]
        public int IdRoomStatus { get; set; }

        public string Title { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
