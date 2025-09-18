using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.CollectableItems
{
    public sealed class CollectableItemCreateRequest
    {
        [Required, MaxLength(200)]
        public string ItemName { get; init; } = default!;
        public DateTime? ItemYear { get; init; }
        [MaxLength(100)]
        public string? ItemNumismat { get; init; }
        [MaxLength(100)]
        public string? ItemValue { get; init; }
        [MaxLength(500)]
        public string? PhotoFilePath { get; init; }
        public DateTime? DateOfAquire { get; init; }
        [Required, Range(1, int.MaxValue)] 
        public int CollectionId { get; init; }
        [MaxLength(50)]
        public string? State { get; init; }
        public string? Description { get; init; }
        public int OwnerId { get; set; }
    }
}
