namespace AnimeVnInfoBackend.Models
{
    public class AnimeListStatus : Base
    {
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int Order { get; set; }

        // List of tables connected
    }
}
