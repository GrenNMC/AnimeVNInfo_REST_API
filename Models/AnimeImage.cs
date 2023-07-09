namespace AnimeVnInfoBackend.Models
{
    public class AnimeImage : Base
    {
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }

        // Foreign keys
        public int? AnimeId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
    }
}
