namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeTypeView
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public int AnimeCount { get; set; }
    }

    public class AnimeTypeParamView : PaginationView
    {

    }

    public class AnimeTypeListView : ResponseWithPaginationView
    {
        public List<AnimeTypeView> Data { get; set; }
        public AnimeTypeListView(List<AnimeTypeView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
