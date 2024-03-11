using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaravanovySvet.Models
{
    public class BlogImage
    {
        public int Id { get; set; } // Primary key

        [DataType(DataType.ImageUrl)]
        public string? ImagePath { get; set; } // Path to the image file
        public string? ImageStorageName { get; set; }

        public required string AltText { get; set; } // Alternative text for the image

        // Foreign key for Blogs
        [ForeignKey("Blog")]
        public int? BlogId { get; set; }

        // Navigation property for the associated blog
        public virtual Blogs? Blog { get; set; }

        [DisplayName("Vybrat fotografii")]
        [NotMapped]
        [Required]
        public virtual IFormFile? ImageFile { get; set; }
    }
}
