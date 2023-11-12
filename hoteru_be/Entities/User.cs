using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public int IdUserType { get; set; }

        [Required]
        [ForeignKey(nameof(IdUserType))]
        public UserType UserType { get; set; }
    }
}
