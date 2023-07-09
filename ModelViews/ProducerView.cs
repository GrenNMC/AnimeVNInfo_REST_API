namespace AnimeVnInfoBackend.ModelViews
{
    public class ProducerView
    {
        public int Id { get; set; }
        public string? ProducerName { get; set; }
        public string? ProducerLink { get; set; }
        public string? Description { get; set; }
        public DateTime? Established { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdAnilist { get; set; }
        public int AnimeCount { get; set; }
        public List<ProducerImageView>? ProducerImage { get; set; }
    }

    public class ProducerParamView : PaginationView
    {
        public string? SearchName { get; set; }
    }

    public class ProducerQueryListView
    {
        public List<int>? Id { get; set; }
    }

    public class ProducerListView : ResponseWithPaginationView
    {
        public List<ProducerView> Data { get; set; }
        public ProducerListView(List<ProducerView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
