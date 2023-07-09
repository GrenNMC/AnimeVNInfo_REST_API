namespace AnimeVnInfoBackend.ModelViews
{
    public class MainNavigationView
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public int ParentId { get; set; }
        public int Order { get; set; }
        public bool IsDisabled { get; set; }
        public List<MainNavigationView>? SubNavigation { get; set; }
    }

    public class MainNavParamView : PaginationView
    {
        public bool? IsDisabled { get; set; }
    }

    public class MainNavListView : ResponseWithPaginationView
    {
        public List<MainNavigationView> Data { get; set; }
        public MainNavListView(List<MainNavigationView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}