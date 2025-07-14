using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class TourPackage
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Duration { get; set; }

        public decimal? Price { get; set; }
        public int? MaxGuests { get; set; }

        [MaxLength(20)]
        public string? Difficulty { get; set; }

        [MaxLength(200)]
        public string? ImageUrl { get; set; }

        public string? HighlightsJson { get; set; }

        [MaxLength(500)]
        public string? IncludedServices { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}