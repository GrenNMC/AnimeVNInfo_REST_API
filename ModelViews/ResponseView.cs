namespace AnimeVnInfoBackend.ModelViews
{
    public class ResponseView
    {
        public string? Message { get; set; }
        public int ErrorCode { get; set; }

        public ResponseView(string? Message, int ErrorCode)
        {
            this.Message = Message;
            this.ErrorCode = ErrorCode;
        }
    }

    public class ResponseWithPaginationView
    {
        public int TotalRecord { get; set; }
        public string? Message { get; set; }
        public int ErrorCode { get; set; }

        public ResponseWithPaginationView(int TotalRecord, int ErrorCode, string? Message)
        {
            this.TotalRecord = TotalRecord;
            this.ErrorCode = ErrorCode;
            this.Message = Message;
        }
    }

    public class ResponseLoginView
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public int ErrorCode { get; set; }

        public ResponseLoginView(int UserId, string? Token, string? Message, int ErrorCode, string? UserName)
        {
            this.UserId = UserId;
            this.Token = Token;
            this.Message = Message;
            this.ErrorCode = ErrorCode;
            this.UserName = UserName;
        }

        public ResponseLoginView(string? Message, int ErrorCode)
        {
            this.Message = Message;
            this.ErrorCode = ErrorCode;
        }
    }
}
