using System.ComponentModel.DataAnnotations;

namespace JEWA_Blog.Models
{
    public class Comment
    {
        [Required]
        public string CommentId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime PublicationTime { get; set; } = DateTime.Now;

        public string Author { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string PostId { get; set; } = string.Empty;

        public string ReplyTo { get; set; } = string.Empty;

        public List<Comment> Replies { get; set; } = new List<Comment>();
    }
}
