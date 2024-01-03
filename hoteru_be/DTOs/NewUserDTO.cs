using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class NewUserDTO
    {
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "Name must be less than 20 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(20, ErrorMessage = "Surname must be less than 20 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Surname must contain only letters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Login name is required")]
        [MaxLength(15, ErrorMessage = "Login name must be less than 15 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Login name must contain only letters")]
        public string LoginName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User type is required")]
        public int IdUserType { get; set; }
    }
}
