namespace AnimeVnInfoBackend.ModelViews
{
    public class AdminNavigationView
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public int ParentId { get; set; }
        public int Order { get; set; }
        public bool IsDisabled { get; set; }
        public bool AdminRole { get; set; }
        public bool ModeratorRole { get; set; }
        public bool UserRole { get; set; }
        public int? RoleId { get; set; }
        public List<AdminNavigationView>? SubNavigation { get; set; }
    }

    public class AdminNavParamView : PaginationView
    {
        public bool? IsDisabled { get; set; }
    }

    public class AdminNavListView : ResponseWithPaginationView
    {
        public List<AdminNavigationView> Data { get; set; }
        public AdminNavListView(List<AdminNavigationView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
