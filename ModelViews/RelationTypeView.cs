namespace AnimeVnInfoBackend.ModelViews
{
    public class RelationTypeView
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }

    public class RelationTypeParamView : PaginationView
    {

    }

    public class RelationTypeListView : ResponseWithPaginationView
    {
        public List<RelationTypeView> Data { get; set; }
        public RelationTypeListView(List<RelationTypeView> Data, int TotalRecord, int ErrorCode, string? Message) : base(TotalRecord, ErrorCode, Message)
        {
            this.Data = Data;
        }
    }
}
