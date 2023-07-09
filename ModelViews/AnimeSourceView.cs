namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeSourceView
    {
        public int Id { get; set; }
        public string? SourceName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public int AnimeCount { get; set; }
    }

    public class AnimeSourceParamView : PaginationView
    {

    }

    public class AnimeSourceListView : ResponseWithPaginationView
    {
        public List<AnimeSourceView> Data { get; set; }
        public AnimeSourceListView(List<AnimeSourceView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
