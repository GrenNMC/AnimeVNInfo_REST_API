namespace AnimeVnInfoBackend.ModelViews
{
    public class LoggerView
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Content { get; set; }
    }

    public class LoggerParamView : PaginationView
    {

    }


    public class LoggerListView : ResponseWithPaginationView
    {
        public List<LoggerView> Data { get; set; }
        public LoggerListView(List<LoggerView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
