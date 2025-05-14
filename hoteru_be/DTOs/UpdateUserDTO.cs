using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class UpdateUserDTO
    {
        public int IdPerson { get; set; }

        [Required, MaxLength(20), RegularExpression(@"^[a-zA-Z]+$")]
        public string Name { get; set; }

        [Required, MaxLength(15), RegularExpression(@"^[a-zA-Z]+$")]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(15), RegularExpression(@"^[a-zA-Z]+$")]
        public string LoginName { get; set; }

        [Required]
        public int IdUserType { get; set; }
    }

}
