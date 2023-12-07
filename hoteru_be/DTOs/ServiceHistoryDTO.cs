using System;

namespace hoteru_be.DTOs
{
    public class ServiceHistoryDTO
    {
        public int IdService { get; set; }
        public string Title { get; set; }     
        public float Sum { get; set; }
        public string Date { get; set; }
    }
}
