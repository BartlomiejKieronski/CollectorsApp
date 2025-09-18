namespace CollectorsApp.Models.DTO.AdminComments
{
    public sealed class AdminCommentResponse
    {
        public int Id { get; init; }
        public int EventLogId { get; init; }
        public string? TargetType { get; init; } = default!;
        public string CommentText { get; init; } = default!;
        public DateTime TimeStamp { get; init; }
        public int AdminId { get; init; }
    }
}
