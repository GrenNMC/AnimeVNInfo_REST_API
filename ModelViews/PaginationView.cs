namespace AnimeVnInfoBackend.ModelViews
{
    public abstract class PaginationView
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? Order { get; set; }
        public string? OrderBy { get; set; }
        public string? SearchString { get; set; }

    }
}
