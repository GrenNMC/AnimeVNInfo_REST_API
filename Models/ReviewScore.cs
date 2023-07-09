namespace AnimeVnInfoBackend.Models
{
    public class ReviewScore : Base
    {
        public int Score { get; set; }
        public string? Description { get; set; }

        // List of tables connected
        public List<Review>? Review { get; set; }
    }
}
