namespace AnimeVnInfoBackend.Models
{
    public class Language : Base
    {
        public string? LanguageName { get; set; }
        public string? Description { get; set; }
        public string? OriginalName { get; set; }

        // List of tables connected
        public List<Staff>? Staff { get; set; }
        public List<Anime>? Anime { get; set; }
    }
}
