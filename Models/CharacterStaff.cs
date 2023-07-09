namespace AnimeVnInfoBackend.Models
{
    public class CharacterStaff : Base
    {
        // Foreign keys
        public int? CharacterId { get; set; }
        public int? StaffId { get; set; }

        // Connect to tables
        public Character? Character { get; set; }
        public Staff? Staff { get; set; }
    }
}
