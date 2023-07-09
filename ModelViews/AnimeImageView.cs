namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeImageView
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }
        public int? AnimeId { get; set; }
    }
}
