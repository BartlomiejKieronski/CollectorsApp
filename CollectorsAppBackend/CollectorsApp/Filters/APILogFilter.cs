namespace CollectorsApp.Filters
{
    public class APILogFilter : BaseFilters
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
        public int? DurationMs { get; set; }
    }
}
