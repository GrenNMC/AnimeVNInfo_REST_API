namespace AnimeVnInfoBackend.Models
{
    public class ProducerImage : Base
    {
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }

        // Foreign keys
        public int? ProducerId { get; set; }

        // Connect to tables
        public Producer? Producer { get; set; }
    }
}
