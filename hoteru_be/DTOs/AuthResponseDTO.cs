using System;

namespace hoteru_be.DTOs
{
    public sealed class AuthResponseDTO
    {
        public string Token { get; set; } = "";
        public string Type { get; set; } = "Bearer";
        public DateTime ExpiresAtUtc { get; set; }
    }
}
