namespace AnimeVnInfoBackend.ModelViews
{
    public class StaffJobView
    {
        public int Id { get; set; }
        public bool IsMain { get; set; }
        public int? JobId { get; set; }
        public int? StaffId { get; set; }
        public string? StaffJobName { get; set; }
    }
}
