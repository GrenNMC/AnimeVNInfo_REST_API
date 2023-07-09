namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeStatusView
    {
        public int Id { get; set; }
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public int AnimeCount { get; set; }
    }

    public class AnimeStatusParamView : PaginationView
    {

    }

    public class AnimeStatusListView : ResponseWithPaginationView
    {
        public List<AnimeStatusView> Data { get; set; }
        public AnimeStatusListView(List<AnimeStatusView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
