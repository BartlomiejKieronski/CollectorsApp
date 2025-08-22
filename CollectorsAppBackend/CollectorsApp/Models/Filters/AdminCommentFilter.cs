namespace CollectorsApp.Models.Filters
{
    public class AdminCommentFilter
    {
        public int? Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public DateTime? Before { get; set; }
        public DateTime? After { get; set; }
        public string? CommentText { get; set; }
        public int? AdminId { get; set; }
        public int? EventLogId { get; set; }
        public string? TargetType { get; set; }
        public int? NumberOfRecords { get; set; }
        public int? Page { get; set; }
        public string? OrderBy { get; set; }
        public bool? SortDescending { get; set; }
    }
}