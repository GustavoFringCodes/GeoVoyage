using System.ComponentModel.DataAnnotations;

namespace GeoVoyage.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Question { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Answer { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}