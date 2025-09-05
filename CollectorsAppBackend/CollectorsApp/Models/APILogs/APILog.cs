using ServiceStack.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CollectorsApp.Models.APILogs
{
    public class APILog
    {
        [NotNull, AutoIncrement]
        public int Id { get; set; }
        [AllowNull]
        public int? UserId { get; set; }
        [AllowNull]
        public string? Controller { get; set; }
        [AllowNull]
        public string? Action { get; set; }
        [AllowNull]
        public int? StatusCode { get; set; }
        [AllowNull]
        public bool? IsSuccess { get; set; }
        [AllowNull]
        public string? Title { get; set; }
        [AllowNull]
        public string? Description { get; set; }
        [AllowNull]
        public string? ErrorMessage { get; set; }
        [AllowNull]
        public string? RequestPath { get; set; }
        [AllowNull]
        public string? HttpMethod { get; set; }
        [AllowNull]
        public string? IpAddress { get; set; }
        [AllowNull]
        public string? IpIV { get; set; }
        [NotNull]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        [AllowNull]
        public int? DurationMs { get; set; }
    }
}
