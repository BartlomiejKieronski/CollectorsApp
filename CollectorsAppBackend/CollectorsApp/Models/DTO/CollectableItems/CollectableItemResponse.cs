namespace CollectorsApp.Models.DTO.CollectableItems
{
    public sealed class CollectableItemResponse
    {
        public int Id { get; init; }
        public string ItemName { get; init; } = default!;
        public DateTime? ItemYear { get; init; }
        public string? ItemNumismat { get; init; }
        public string? ItemValue { get; init; }
        public string? PhotoFilePath { get; init; }
        public DateTime? DateOfAquire { get; init; }
        public int CollectionId { get; init; }
        public string? State { get; init; }
        public string? Description { get; init; }
        public DateTime? LastUpdated { get; init; }
        public DateTime? TimeStamp { get; init; }
        public int OwnerId { get; init; }
    }
}
