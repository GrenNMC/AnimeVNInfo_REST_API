namespace AnimeVnInfoBackend.Models
{
    public class AnimeProducer : Base
    {
        // Foreign keys
        public int? AnimeId { get; set; }
        public int? ProducerId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public Producer? Producer { get; set; }
    }
}
