namespace AnimeVnInfoBackend.Models
{
    public class StaffJob : Base
    {
        public bool IsMain { get; set; }

        // Foreign keys
        public int? JobId { get; set; }
        public int? StaffId { get; set; }

        // Connect to tables
        public Job? Job { get; set; }
        public Staff? Staff { get; set; }
    }
}
