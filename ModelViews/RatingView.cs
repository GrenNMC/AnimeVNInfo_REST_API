namespace AnimeVnInfoBackend.ModelViews
{
    public class RatingView
    {
        public int Id { get; set; }
        public string? RatingName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public int AnimeCount { get; set; }
    }

    public class RatingParamView : PaginationView
    {

    }

    public class RatingListView : ResponseWithPaginationView
    {
        public List<RatingView> Data { get; set; }
        public RatingListView(List<RatingView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
