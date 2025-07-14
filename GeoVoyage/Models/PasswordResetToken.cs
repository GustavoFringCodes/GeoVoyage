using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }

        [Required]
        public int CustomerAccountId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public CustomerAccount CustomerAccount { get; set; }
    }
}