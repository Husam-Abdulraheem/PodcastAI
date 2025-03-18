using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PodcastAI.Models
{
    public class Podcast
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Subject { get; set; }
        public string? Size { get; set; }
        public string? Content { get; set; }
        public string? AudioUrl { get; set; }
        public string? ImageUrl { get; set; }
    }
}
