using System;

namespace hoteru_be.DTOs
{
    public class ServiceHistoryDTO
    {
        public int IdService { get; set; }
        public string Title { get; set; }     
        public decimal Sum { get; set; }
        public DateTime Date { get; set; }
    }
}
