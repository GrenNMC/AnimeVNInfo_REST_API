namespace AnimeVnInfoBackend.ModelViews
{
    public class StaffView
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public int IdAnilist { get; set; }
        public int? LanguageId { get; set; }
        public string? LanguageName { get; set; }
        public int CharacterCount { get; set; }
        public List<StaffImageView>? StaffImage { get; set; }
        public List<StaffJobView>? StaffJob { get; set; }
    }

    public class StaffParamView : PaginationView
    {
        public string? SearchName { get; set; }
        public int? LanguageId { get; set; }
        public int? DayOfBirth { get; set; }
        public int? MonthOfBirth { get; set; }
        public int? YearOfBirth { get; set; }
        public int? DayOfDeath { get; set; }
        public int? MonthOfDeath { get; set; }
        public int? YearOfDeath { get; set; }
        public List<int>? JobId { get; set; }
        public bool? IsJobGroup { get; set; }
    }

    public class StaffQueryListView
    {
        public List<int>? Id { get; set; }
    }

    public class StaffListView : ResponseWithPaginationView
    {
        public List<StaffView> Data { get; set; }
        public StaffListView(List<StaffView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
