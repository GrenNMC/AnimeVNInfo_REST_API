namespace AnimeVnInfoBackend.Models
{
    public class AnimeGenre : Base
    {
        // Foreign keys
        public int? GenreId { get; set; }
        public int? AnimeId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public Genre? Genre { get; set; }
    }
}
