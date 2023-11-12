using System.ComponentModel.DataAnnotations;

namespace hoteru_be.Entities
{
    public class UserType
    {
        [Key]
        public int IdUserType { get; set; }
        public string Title { get; set; }
        public User User { get; set; }
    }
}
