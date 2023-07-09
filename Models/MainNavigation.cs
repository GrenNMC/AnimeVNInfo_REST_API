namespace AnimeVnInfoBackend.Models
{
    public class MainNavigation : Base
    {
        public string? Title { get; set; }
        public string? Link { get; set; }
        public int ParentId { get; set; }
        public int Order { get; set; }
        public bool IsDisabled { get; set; }
    }
}
