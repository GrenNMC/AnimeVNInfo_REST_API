namespace AnimeVnInfoBackend.Models
{
    public class Genre : Base
    {
        public string? GenreName { get; set; }
        public string? Description { get; set; }

        // List of tables connected
        public List<AnimeGenre>? AnimeGenre { get; set; }
    }
}
