namespace CollectorsApp.Models.DTO.Collections
{
    public sealed class CollectionResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public int ParentId { get; init; }
        public string? ParentName { get; init; }
        public int Depth { get; init; }
        public DateTime TimeStamp { get; init; }
        public DateTime? LastUpdated { get; init; }
    }
}
