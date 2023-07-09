namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeGenreView
    {
        public int Id { get; set; }
        public int? GenreId { get; set; }
        public int? AnimeId { get; set; }
        public string? GenreName { get; set;}
    }
}
