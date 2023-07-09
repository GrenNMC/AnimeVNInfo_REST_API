namespace AnimeVnInfoBackend.ModelViews
{
    public class LanguageView
    {
        public int Id { get; set; }
        public string? LanguageName { get; set; }
        public string? Description { get; set; }
        public string? OriginalName { get; set; }
        public int StaffCount { get; set; }
    }

    public class LanguageParamView : PaginationView
    {

    }

    public class LanguageListView : ResponseWithPaginationView
    {
        public List<LanguageView> Data { get; set; }
        public LanguageListView(List<LanguageView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
