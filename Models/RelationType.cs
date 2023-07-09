namespace AnimeVnInfoBackend.Models
{
    public class RelationType : Base
    {
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }

        // List of tables connected
        public List<AnimeRelation>? AnimeRelation { get; set; }
    }
}
