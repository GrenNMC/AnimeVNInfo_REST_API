namespace AnimeVnInfoBackend.Models
{
    public class Character : Base
    {
        public string? NativeName { get; set; }
        public string? LatinName { get; set; }
        public string? OtherName { get; set; }
        public bool? IsMale { get; set; }
        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }
        public string? Age { get; set; }
        public string? BloodType { get; set; }
        public string? OtherInformation { get; set; }
        public int IdAnilist { get; set; }

        // List of tables connected
        public List<AnimeCharacter>? AnimeCharacter { get; set; }
        public List<CharacterImage>? CharacterImage { get; set; }
        public List<CharacterStaff>? CharacterStaff { get; set; }
    }
}
