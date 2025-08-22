namespace CollectorsApp.Models.Filters
{
    public class APILogFilter
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccess { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RequestPath { get; set; }
        public string? HttpMethod { get; set; }
        public string? IpAddress { get; set; }
        public string? IpIV { get; set; }
        public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;
        public DateTime? After { get; set; }
        public DateTime? Before { get; set; }
        public string? OrderBy { get; set; }
        public string? SortBy { get; set; }
        public int? Page { get; set; }
        public int? NumberOfItems { get; set; }
        public int? DurationMs { get; set; }
    }
}
