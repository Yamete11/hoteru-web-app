namespace hoteru_be.DTOs
{
    public class NewUserDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int IdUserType { get; set; }
    }
}
