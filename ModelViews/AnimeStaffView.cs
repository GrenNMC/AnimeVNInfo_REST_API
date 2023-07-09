namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeStaffView
    {
        public int Id { get; set; }
        public string? Role { get; set; }
        public int? AnimeId { get; set; }
        public int? StaffId { get; set; }
        public string? StaffNativeName { get; set; }
        public string? StaffLatinName { get; set; }
        public string? StaffLanguage { get; set; }
        public int? StaffIdAnilist { get; set; }
    }
}
