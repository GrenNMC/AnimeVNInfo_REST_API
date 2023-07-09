namespace AnimeVnInfoBackend.Models
{
    public class AnimeRelation : Base
    {
        // Foreign keys
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public int? RelationTypeId { get; set; }

        // Connect to tables
        public Anime? ParentAnime { get; set; }
        public Anime? ChildAnime { get; set; }
        public RelationType? RelationType { get; set; }
    }
}
