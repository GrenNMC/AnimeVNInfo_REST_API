namespace AnimeVnInfoBackend.Models
{
    public class StudioImage : Base
    {
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }

        // Foreign keys
        public int? StudioId { get; set; }

        // Connect to tables
        public Studio? Studio { get; set; }
    }
}