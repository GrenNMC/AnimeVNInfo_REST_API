namespace AnimeVnInfoBackend.Models
{
    public class UserInfo : Base
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }

        // Foreign keys
        public int? UserId { get; set; }

        // Connect to tables
        public User? User { get; set; }
    }
}
