namespace AnimeVnInfoBackend.ModelViews
{
    public class UserView
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class UserParamView : PaginationView
    {

    }


    public class UserListView : ResponseWithPaginationView
    {
        public List<UserView> Data { get; set; }
        public UserListView(List<UserView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }

    public class LoginView
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Code { get; set; }
    }

    public class UserRolesAndDisableStatusView
    {
        public bool AdminRole { get; set; }
        public bool ModeratorRole { get; set; }
        public bool UserRole { get; set; }
        public bool IsDisabled { get; set; }
    }
}
