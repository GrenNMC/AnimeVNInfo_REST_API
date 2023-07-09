namespace AnimeVnInfoBackend.ModelViews
{
    public class CharacterView
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public int AnimeCount { get; set; }
        public List<CharacterStaffView>? CharacterStaff { get; set; }
        public List<CharacterImageView>? CharacterImage { get; set; }
    }

    public class CharacterParamView : PaginationView
    {
        public int? DayOfBirth { get; set; }
        public int? MonthOfBirth { get; set; }
        public int? YearOfBirth { get; set; }
        public string? SearchName { get; set; }
        public string? SearchStaff { get; set; }
    }

    public class CharacterQueryListView
    {
        public List<int>? Id { get; set; }
    }

    public class CharacterListView : ResponseWithPaginationView
    {
        public List<CharacterView> Data { get; set; }
        public CharacterListView(List<CharacterView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
