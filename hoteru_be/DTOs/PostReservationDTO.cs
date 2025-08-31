using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class PostReservationDTO
    {
        [Required(ErrorMessage = "Check-in date is required")]
        [DataType(DataType.Date)]
        public DateTime In { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        [DataType(DataType.Date)]
        public DateTime Out { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }

        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100 people")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Room ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Room ID must be a positive number")]
        public int IdRoom { get; set; }

        public bool Confirmed { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "Total amount cannot be negative")]
        public decimal Sum { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Deposit type must be zero or a positive number")]
        public int IdDepositType { get; set; }

        public List<ServiceHistoryDTO> Services { get; set; } = new();

        [Required(ErrorMessage = "Person ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Person ID must be a positive number")]
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive number")]
        public int IdUser { get; set; }
    }
}
