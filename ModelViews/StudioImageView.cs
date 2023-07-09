namespace AnimeVnInfoBackend.ModelViews
{
    public class StudioImageView
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }
        public int? StudioId { get; set; }
    }
}
