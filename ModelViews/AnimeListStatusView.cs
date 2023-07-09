namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeListStatusView
    {
        public int Id { get; set; }
        public string? StatusName { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public int Order { get; set; }
    }

    public class AnimeListStatusParamView : PaginationView
    {

    }

    public class AnimeListStatusListView : ResponseWithPaginationView
    {
        public List<AnimeListStatusView> Data { get; set; }
        public AnimeListStatusListView(List<AnimeListStatusView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
