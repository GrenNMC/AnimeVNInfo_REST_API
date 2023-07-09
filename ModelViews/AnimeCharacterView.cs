namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeCharacterView
    {
        public int Id { get; set; }
        public bool? IsMain { get; set; }
        public int? CharacterId { get; set; }
        public int? AnimeId { get; set; }
        public string? CharacterNativeName { get; set; }
        public string? CharacterLatinName { get; set; }
        public int? CharacterIdAnilist { get; set; }
    }
}
