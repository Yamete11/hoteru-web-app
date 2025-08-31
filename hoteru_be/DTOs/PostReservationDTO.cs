using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class PostReservationDTO
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime In { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Out { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal Price { get; set; }

        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        public int Capacity { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "IdRoom must be a positive integer")]
        public int IdRoom { get; set; }

        public bool Confirmed { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "Sum must be greater than or equal to 0")]
        public decimal Sum { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "IdDepositType must be 0 or a positive integer")]
        public int IdDepositType { get; set; }

        public List<ServiceHistoryDTO> Services { get; set; } = new();

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "IdPerson must be a positive integer")]
        public int IdPerson { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "IdUser must be a positive integer")]
        public int IdUser { get; set; }
    }
}
