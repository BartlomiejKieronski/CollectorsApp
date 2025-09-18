using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.Analytics
{
    public class AdminComment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AdminId { get; set; }
        [Required]
        public int EventLogId { get; set; }
        [Required]
        [MaxLength(50)]
        public string TargetType { get; set; }
        [Required]
        public string CommentText { get; set; }
        public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
