using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class HotelDTO
    {
        [Required]
        [StringLength(20, ErrorMessage = "Name must be less than 20 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Surname must be less than 20 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Surname must contain only letters")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email is not properly written")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Login name must be less than 20 characters")]
        public string LoginName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Password must be less than 20 characters")]
        public string Password { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Title must be less than 20 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Title must contain only letters")]
        public string Title { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "City must be less than 20 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "City must contain only letters")]
        public string City { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Country must be less than 20 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Country must contain only letters")]
        public string Country { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Street must be less than 20 characters")]
        public string Street { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Postcode must be less than 15 characters")]
        public string Postcode { get; set; }
    }
}
