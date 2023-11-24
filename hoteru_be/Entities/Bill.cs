using System;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class Bill
    {
        [Key]
        public int IdBill { get; set; }
        public DateTime Creating { get; set; }
        public float Sum { get; set; }

        public Reservation Reservation { get; set; }
    }
}
