namespace AnimeVnInfoBackend.ModelViews
{
    public class CharacterImageView
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }
        public int? CharacterId { get; set; }
    }
}
