namespace AnimeVnInfoBackend.Models
{
    public class AnimeStudio : Base
    {
        // Foreign keys
        public int? AnimeId { get; set; }
        public int? StudioId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public Studio? Studio { get; set; }
    }
}
