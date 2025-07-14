using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class NewsletterSubscription
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}