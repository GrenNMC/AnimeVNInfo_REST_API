namespace AnimeVnInfoBackend.Models
{
    public class AdminNavigation : Base
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
        public int ParentId { get; set; }
        public int Order { get; set; }
        public bool IsDisabled { get; set; }

        // Foreign keys
        public int? RoleId { get; set; }

        // Connect to tables
        public Role? Role { get; set; }
    }
}
