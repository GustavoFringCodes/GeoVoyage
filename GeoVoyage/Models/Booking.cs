using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int? CustomerAccountId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        public int? DestinationId { get; set; }

        [MaxLength(50)]
        public string? PackageType { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? NumberOfGuests { get; set; }
        public decimal? TotalPrice { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? SpecialRequests { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public CustomerAccount? CustomerAccount { get; set; }
    }
}