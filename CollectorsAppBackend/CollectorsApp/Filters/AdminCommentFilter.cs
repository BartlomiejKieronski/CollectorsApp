namespace CollectorsApp.Filters
{
    public class AdminCommentFilter : BaseFilters
    {
        public int? Id { get; set; }
        public string? CommentText { get; set; }
        public int? AdminId { get; set; }
        public int? EventLogId { get; set; }
        public string? TargetType { get; set; }
    }
}