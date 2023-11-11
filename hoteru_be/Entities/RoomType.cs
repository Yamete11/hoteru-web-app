using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class RoomType
    {
        [Key]
        public int IdRoomType { get; set; }
        public string Title { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
