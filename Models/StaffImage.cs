namespace AnimeVnInfoBackend.Models
{
    public class StaffImage : Base
    {
        public string? Image { get; set; }
        public bool IsAvatar { get; set; }

        // Foreign keys
        public int? StaffId { get; set; }

        // Connect to tables
        public Staff? Staff { get; set; }
    }
}
