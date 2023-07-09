namespace AnimeVnInfoBackend.Models
{
    public class Season : Base
    {
        public int Year { get; set; }
        public int Quarter { get; set; }

        // List of tables connected
        public List<Anime>? Anime { get; set; }
    }
}
