using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models.Analytics
{
    public class AdminComment
    {
        [NotNull,AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int AdminId { get; set; }
        [NotNull]
        public int EventLogId { get; set; }
        [NotNull]
        public string TargetType { get; set; }
        [NotNull]
        public string CommentText { get; set; }
        [AllowNull]
        public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
