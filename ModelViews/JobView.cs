namespace AnimeVnInfoBackend.ModelViews
{
    public class JobView
    {
        public int Id { get; set; }
        public string? JobName { get; set; }
        public string? Description { get; set; }
        public int StaffCount { get; set; }
    }

    public class JobParamView : PaginationView
    {

    }

    public class JobListView : ResponseWithPaginationView
    {
        public List<JobView> Data { get; set; }
        public JobListView(List<JobView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
