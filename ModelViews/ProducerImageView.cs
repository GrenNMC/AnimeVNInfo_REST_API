namespace AnimeVnInfoBackend.ModelViews
{
    public class ProducerImageView
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }
        public int? ProducerId { get; set; }
    }
}
