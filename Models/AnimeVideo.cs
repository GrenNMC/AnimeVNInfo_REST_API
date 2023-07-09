namespace AnimeVnInfoBackend.Models
{
    public class AnimeVideo : Base
    {
        public string? Link { get; set; }

        // Foreign keys
        public int? AnimeId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
    }
}
