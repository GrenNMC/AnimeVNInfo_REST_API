namespace AnimeVnInfoBackend.ModelViews
{
    public class ReviewScoreView
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string? Description { get; set; }
    }

    public class DeletedReviewScoreView : ReviewScoreView {
        public DateTime? DeletedAt { get; set; }
    }

    public class ReviewScoreParamView : PaginationView
    {

    }

    public class ReviewScoreListView : ResponseWithPaginationView
    {
        public List<ReviewScoreView> Data { get; set; }
        public ReviewScoreListView(List<ReviewScoreView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }

    public class DeletedReviewScoreListView : ResponseWithPaginationView {
        public List<DeletedReviewScoreView> Data { get; set; }
        public DeletedReviewScoreListView(List<DeletedReviewScoreView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message) {
            this.Data = Data;
        }
    }
}
