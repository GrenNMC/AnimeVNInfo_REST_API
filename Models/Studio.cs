namespace AnimeVnInfoBackend.Models
{
    public class Studio : Base
    {
        public string? StudioName { get; set; }
        public string? StudioLink { get; set; }
        public string? Description { get; set; }
        public DateTime? Established { get; set; }
        public int IdAnilist { get; set; }

        // List of tables connected
        public List<AnimeStudio>? AnimeStudio { get; set; }
        public List<StudioImage>? StudioImage { get; set; }
    }
}
