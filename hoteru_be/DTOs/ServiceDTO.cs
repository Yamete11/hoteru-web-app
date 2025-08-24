using System.ComponentModel.DataAnnotations;

namespace hoteru_be.DTOs
{
    public class ServiceDTO
    {
        public int IdService { get; set; }

        private string? _title;
        private string? _description;

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(20, ErrorMessage = "Title can have max 20 characters.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Title cannot be only whitespace.")]
        public string Title
        {
            get => _title ?? string.Empty;
            set => _title = value?.Trim();
        }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1_000_000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
        public decimal? Sum { get; set; }

        [MaxLength(50, ErrorMessage = "Description can have max 50 characters.")]
        public string? Description
        {
            get => _description;
            set => _description = value?.Trim();
        }
    }
}
