namespace AnimeVnInfoBackend.Models
{
    public class Job : Base
    {
        public string? JobName { get; set; }
        public string? Description { get; set; }

        // List of tables connected
        public List<StaffJob>? StaffJob { get; set; }
    }
}
