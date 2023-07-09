namespace AnimeVnInfoBackend.ModelViews
{
    public class AnimeRelationView
    {
        public int Id { get; set; }
        public int? RelationTypeId { get; set; }
        public int? ParentId { get; set; }
        public int? ChildId { get; set; }
        public string? ParentNativeName { get; set; }
        public string? ParentLatinName { get; set; }
        public string? ChildNativeName { get; set; }
        public string? ChildLatinName { get; set; }
    }
}
