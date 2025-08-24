using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class UpdateUserDTO
    {
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(20, ErrorMessage = "Name must be less than 20 characters")]
        [RegularExpression(@"^\p{L}+(?:[ '-]\p{L}+)*$", ErrorMessage = "Name contains invalid characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(20, ErrorMessage = "Surname must be less than 20 characters")]
        [RegularExpression(@"^\p{L}+(?:[ '-]\p{L}+)*$", ErrorMessage = "Surname contains invalid characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Login name is required")]
        [MinLength(3, ErrorMessage = "Login name must be at least 3 characters")]
        [MaxLength(15, ErrorMessage = "Login name must be less than 15 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Login name can contain letters, digits, dot, underscore and hyphen")]
        public string LoginName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "User type is invalid")]
        public int IdUserType { get; set; }
    }
}
