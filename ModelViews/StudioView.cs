namespace AnimeVnInfoBackend.ModelViews
{
    public class StudioView
    {
        public int Id { get; set; }
        public string? StudioName { get; set; }
        public string? StudioLink { get; set; }
        public string? Description { get; set; }
        public DateTime? Established { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdAnilist { get; set; }
        public int AnimeCount { get; set; }
        public List<StudioImageView>? StudioImage { get; set; }
    }

    public class StudioParamView : PaginationView
    {
        public string? SearchName { get; set; }
    }

    public class StudioQueryListView
    {
        public List<int>? Id { get; set; }
    }

    public class StudioListView : ResponseWithPaginationView
    {
        public List<StudioView> Data { get; set; }
        public StudioListView(List<StudioView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
