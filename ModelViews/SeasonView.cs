namespace AnimeVnInfoBackend.ModelViews
{
    public class SeasonView
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public int AnimeCount { get; set; }
    }

    public class SeasonParamView : PaginationView
    {

    }

    public class SeasonListView : ResponseWithPaginationView
    {
        public List<SeasonView> Data { get; set; }
        public SeasonListView(List<SeasonView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
