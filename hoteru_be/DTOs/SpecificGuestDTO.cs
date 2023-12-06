namespace hoteru_be.DTOs
{
    public class SpecificGuestDTO
    {
        public int IdPerson { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Passport { get; set; }
        public string TelNumber { get; set; }
        public int IdGuestStatus { get; set; }
    }
}
