using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KaravanovySvet.Models
{
    public class BlogImage
    {
        public int Id { get; set; } // Primary key

        [Required]
        public required string ImagePath { get; set; } // Path to the image file

        public required string AltText { get; set; } // Alternative text for the image

        // Foreign key for Blogs
        [ForeignKey("Blog")]
        public int BlogId { get; set; }

        // Navigation property for the associated blog
        public required virtual Blogs Blog { get; set; }
    }
}
