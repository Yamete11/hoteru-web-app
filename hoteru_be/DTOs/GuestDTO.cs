using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class GuestDTO
    {
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "Name must be less than 20 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(20, ErrorMessage = "Surname must be less than 20 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passport number is required")]
        [MaxLength(10, ErrorMessage = "Passport number must be less than 10 characters")]
        public string Passport { get; set; }

        [Required(ErrorMessage = "Telephone number is required")]
        [MaxLength(15, ErrorMessage = "Telephone number must be less than 15 characters")]
        public string TelNumber { get; set; }

        [Required(ErrorMessage = "Guest status is required")]
        public string IdGuestStatus { get; set; }
    }
}
