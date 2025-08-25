using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteru_be.Entities
{
    public class RefreshToken
    {
        [Key]
        public int IdToken { get; set; }

        [ForeignKey(nameof(Person))]
        public int IdPerson { get; set; }
        public Person Person { get; set; } = null!;


        [Required]
        [MaxLength(256)]
        public string TokenHash { get; set; } = null!;

        public DateTime CreatedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }

        public DateTime? RevokedUtc { get; set; }

        [MaxLength(256)]
        public string? ReplacedByTokenHash { get; set; }

        [MaxLength(64)]
        public string? CreatedByIp { get; set; }

        [MaxLength(256)]
        public string? UserAgent { get; set; }

        [NotMapped]
        public bool IsActive => RevokedUtc == null && DateTime.UtcNow < ExpiresUtc;
    }
}
