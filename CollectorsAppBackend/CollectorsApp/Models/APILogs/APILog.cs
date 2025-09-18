using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.APILogs
{
    public class APILog
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        [MaxLength(100)]
        public string? Controller { get; set; }
        [MaxLength(100)]
        public string? Action { get; set; }
        public int? StatusCode { get; set; }
        public bool? IsSuccess { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [MaxLength(2000)]
        public string? ErrorMessage { get; set; }
        [MaxLength(500)]
        public string? RequestPath { get; set; }
        [MaxLength(20)]
        public string? HttpMethod { get; set; }
        [MaxLength(500)]
        public string? IpAddress { get; set; }
        [MaxLength(64)]
        public string? IpIV { get; set; }
        public DateTime TimeStamp { get; set; }
        public int? DurationMs { get; set; }
    }
}
