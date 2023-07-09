namespace AnimeVnInfoBackend.Models
{
    public class AnimeStaff : Base
    {
        public string? Role { get; set; }

        // Foreign keys
        public int? AnimeId { get; set; }
        public int? StaffId { get; set; }

        // Connect to tables
        public Anime? Anime { get; set; }
        public Staff? Staff { get; set; }
    }
}
