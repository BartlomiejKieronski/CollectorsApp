namespace CollectorsApp.Models.DTO.APILogs
{
    public sealed class APILogResponse
    {
        public int Id { get; init; }
        public int? UserId { get; init; }
        public string? Controller { get; init; }
        public string? Action { get; init; }
        public int? StatusCode { get; init; }
        public bool? IsSuccess { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? ErrorMessage { get; init; }
        public string? RequestPath { get; init; }
        public string? HttpMethod { get; init; }
        public DateTime? TimeStamp { get; init; }
        public int? DurationMs { get; init; }
    }
}
