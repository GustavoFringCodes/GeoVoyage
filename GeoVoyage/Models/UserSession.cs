using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class UserSession
    {
        public int Id { get; set; }

        [Required]
        public int CustomerAccountId { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionToken { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public CustomerAccount CustomerAccount { get; set; }
    }
}