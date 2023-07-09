namespace AnimeVnInfoBackend.Models
{
    public class Anime : Base
    {
        public string? NativeName { get; set; }
        public string? LatinName { get; set; }
        public string? OtherName { get; set; }
        public string? Summary { get; set; }
        public string? OpeningSong { get; set; }
        public string? EndingSong { get; set; }
        public bool IsAdult { get; set; }
        public int DayOfStartTime { get; set; }
        public int MonthOfStartTime { get; set; }
        public int YearOfStartTime { get; set; }
        public int DayOfEndTime { get; set; }
        public int MonthOfEndTime { get; set; }
        public int YearOfEndTime { get; set; }
        public int Episodes { get; set; }
        public int HoursDuration { get; set; }
        public int MinutesDuration { get; set; }
        public int SecondsDuration { get; set; }
        public float Score { get; set; }
        public int IdAnilist { get; set; }

        // Foreign keys
        public int? AnimeTypeId { get; set; }
        public int? AnimeStatusId { get; set; }
        public int? AnimeSourceId { get; set; }
        public int? SeasonId { get; set; }
        public int? RatingId { get; set; }
        public int? LanguageId { get; set; }

        // Connect to tables
        public AnimeType? AnimeType { get; set; }
        public AnimeStatus? AnimeStatus { get; set; }
        public AnimeSource? AnimeSource { get; set; }
        public Season? Season { get; set; }
        public Rating? Rating { get; set; }
        public Language? Language { get; set; }

        // List of tables connected
        public List<AnimeImage>? AnimeImage { get; set; }
        public List<AnimeCharacter>? AnimeCharacter { get; set; }
        public List<AnimeGenre>? AnimeGenre { get; set; }
        public List<AnimeProducer>? AnimeProducer { get; set; }
        public List<AnimeStudio>? AnimeStudio { get; set; }
        public List<Review>? Review { get; set; }
        public List<AnimeRelation>? ParentAnimeRelation { get; set; }
        public List<AnimeRelation>? ChildAnimeRelation { get; set; }
        public List<AnimeStaff>? AnimeStaff { get; set; }
        public List<AnimeVideo>? AnimeVideo { get; set; }
    }
}
