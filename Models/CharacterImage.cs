namespace AnimeVnInfoBackend.Models
{
    public class CharacterImage : Base
    {
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }

        // Foreign keys
        public int? CharacterId { get; set; }

        // Connect to tables
        public Character? Character { get; set; }
    }
}
