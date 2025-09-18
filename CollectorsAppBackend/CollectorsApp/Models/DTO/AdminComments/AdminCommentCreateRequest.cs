using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.AdminComments
{
    public sealed class AdminCommentCreateRequest
    {
        [Required]
        public int EventLogId { get; init; }
        [Required, MaxLength(50)] 
        public string TargetType { get; init; } = default!;
        [Required]
        public string CommentText { get; init; } = default!;
    }
}
