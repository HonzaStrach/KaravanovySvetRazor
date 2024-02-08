using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // For the List<BlogImage>

namespace KaravanovySvet.Models
{
    public class Blogs
    {
        public int Id { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Perex { get; set; }
        [Required]
        public required string MainText { get; set; }
        public string? Labels { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }
        public string? PermaLink { get; set; }
        public string? Location { get; set; }
        public string? Keywords { get; set; }
        [Required]
        public required bool Comments { get; set; }

        public int? TotalViews { get; set; }

        // Navigation property for related images
        public virtual List<BlogImage> Images { get; set; } = new List<BlogImage>();

        [NotMapped] // This ensures EF Core does not try to map this property to a database column
        public string[] LabelArray => string.IsNullOrEmpty(Labels) ? new string[0] : Labels.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
