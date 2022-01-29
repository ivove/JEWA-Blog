using System.ComponentModel.DataAnnotations;

namespace JEWA_Blog.Models
{
    public class Post
    {
        [Required]
        public string PostId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Excerpt { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = false;

        public DateTime? PublicationDate { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
