namespace AnimeVnInfoBackend.Models
{
    public class Producer : Base
    {
        public string? ProducerName { get; set; }
        public string? ProducerLink { get; set; }
        public string? Description { get; set; }
        public DateTime? Established { get; set; }
        public int IdAnilist { get; set; }

        // List of tables connected
        public List<AnimeProducer>? AnimeProducer { get; set; }
        public List<ProducerImage>? ProducerImage { get; set; }
    }
}
