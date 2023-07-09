namespace AnimeVnInfoBackend.Models
{
    public class AnimeSource : Base
    {
        public string? SourceName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        // List of tables connected
        public List<Anime>? Anime { get; set; }
    }
}
