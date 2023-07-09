namespace AnimeVnInfoBackend.ModelViews
{
    public class GenreView
    {
        public int Id { get; set; }
        public string? GenreName { get; set; }
        public string? Description { get; set; }
        public int AnimeCount { get; set; }
    }

    public class GenreParamView : PaginationView
    {

    }

    public class GenreListView : ResponseWithPaginationView
    {
        public List<GenreView> Data { get; set; }
        public GenreListView(List<GenreView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
