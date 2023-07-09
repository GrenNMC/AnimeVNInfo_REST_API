namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeView
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public int? AnimeTypeId { get; set; }
        public int? AnimeStatusId { get; set; }
        public int? AnimeSourceId { get; set; }
        public int? SeasonId { get; set; }
        public int? RatingId { get; set; }
        public int? LanguageId { get; set; }
        public string? TypeName { get; set; }
        public string? StatusName { get; set; }
        public string? SourceName { get; set; }
        public string? RatingName { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
        public List<AnimeImageView>? AnimeImage { get; set; }
        public List<AnimeCharacterView>? AnimeCharacter { get; set; }
        public List<AnimeGenreView>? AnimeGenre { get; set; }
        public List<AnimeProducerView>? AnimeProducer { get; set; }
        public List<AnimeStudioView>? AnimeStudio { get; set; }
        public List<AnimeRelationView>? ParentAnimeRelation { get; set; }
        public List<AnimeRelationView>? ChildAnimeRelation { get; set; }
        public List<AnimeStaffView>? AnimeStaff { get; set; }
        public List<AnimeVideoView>? AnimeVideo { get; set; }
    }

    public class AnimeParamView : PaginationView
    {
        public string? SearchName { get; set; }
        public string? SearchStaff { get; set; }
        public string? SearchCharacter { get; set; }
        public int? AnimeTypeId { get; set; }
        public int? AnimeStatusId { get; set; }
        public int? AnimeSourceId { get; set; }
        public int? SeasonId { get; set; }
        public int? RatingId { get; set; }
        public List<int>? StudioId { get; set; }
        public List<int>? ProducerId { get; set; }
        public List<int>? GenreId { get; set; }
        public bool? IsStudioGroup { get; set; }
        public bool? IsProducerGroup { get; set; }
        public bool? IsGenreGroup { get; set; }
        public int? Year { get; set;}
        public int? Quarter { get; set;}
    }

    public class AnimeQueryListView
    {
        public List<int>? Id { get; set; }
    }

    public class AnimeListView : ResponseWithPaginationView
    {
        public List<AnimeView> Data { get; set; }
        public AnimeListView(List<AnimeView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }

    public class HomeAnimeView
    {
        public int Id { get; set; }
        public string? NativeName { get; set; }
        public string? LatinName { get; set; }
        public string? Summary { get; set; }
        public string? TypeName { get; set; }
        public bool IsAdult { get; set; }
        public int DayOfStartTime { get; set; }
        public int MonthOfStartTime { get; set; }
        public int YearOfStartTime { get; set; }
        public int Episodes { get; set; }
        public int HoursDuration { get; set; }
        public int MinutesDuration { get; set; }
        public int SecondsDuration { get; set; }
        public float Score { get; set; }
        public int? AnimeTypeId { get; set; }
        public int? SeasonId { get; set; }
        public int? Year { get; set; }
        public int? Quarter { get; set; }
        public List<AnimeImageView>? AnimeImage { get; set; }
        public List<AnimeGenreView>? AnimeGenre { get; set; }
		public List<AnimeStudioView>? AnimeStudio { get; set; }
	}

    public class HomeAnimeListView : ResponseWithPaginationView
    {
        public List<HomeAnimeView> Data { get; set; }
        public HomeAnimeListView(List<HomeAnimeView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
