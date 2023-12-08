using System.Collections.Generic;

namespace hoteru_be.DTOs
{
    public class PaginatedRoomResultDTO
    {
        public IEnumerable<RoomDTO> Rooms { get; set; }


        public int TotalCount { get; set; }


        public int Page { get; set; }


        public int Limit { get; set; }
    }
}
