namespace AnimeVnInfoBackend.ModelViews
{
    public class CharacterStaffView
    {
        public int Id { get; set; }
        public string? StaffNativeName { get; set; }
        public string? StaffLatinName { get; set; }
        public string? StaffLanguage { get; set; }
        public int? CharacterId { get; set; }
        public int? StaffId { get; set; }
        public int? StaffIdAnilist { get; set; }
    }
}
