namespace AnimeVnInfoBackend.Models
{
    public class Staff : Base
    {
        public string? NativeName { get; set; }
        public string? LatinName { get; set; }
        public string? OtherName { get; set; }
        public bool? IsMale { get; set; }
        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }
        public int DayOfDeath { get; set; }
        public int MonthOfDeath { get; set; }
        public int YearOfDeath { get; set; }
        public string? BloodType { get; set; }
        public string? HomeTown { get; set; }
        public string? OtherInformation { get; set; }
        public int IdAnilist { get; set; }

        // Foreign keys
        public int? LanguageId { get; set; }

        // Connect to tables
        public Language? Language { get; set; }

        // List of tables connected
        public List<CharacterStaff>? CharacterStaff { get; set; }
        public List<StaffImage>? StaffImage { get; set; }
        public List<StaffJob>? StaffJob { get; set; }
        public List<AnimeStaff>? AnimeStaff { get; set; }
    }
}
