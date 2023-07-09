namespace AnimeVnInfoBackend.Models
{
    public class Review : Base
    {
        public string? Content { get; set; }

        // Foreign keys
        public int? AnimeId { get; set; }
        public int? ReviewScoreId { get; set; }
        public int? UserId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public ReviewScore? ReviewScore { get; set; }
        public User? User { get; set; }
    }
}
