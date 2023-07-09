namespace AnimeVnInfoBackend.Models
{
    public class AnimeCharacter : Base
    {
        public bool? IsMain { get; set; }

        // Foreign keys
        public int? AnimeId { get; set; }
        public int? CharacterId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public Character? Character { get; set; }
    }
}
