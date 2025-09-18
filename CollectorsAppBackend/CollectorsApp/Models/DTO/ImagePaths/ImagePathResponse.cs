namespace CollectorsApp.Models.DTO.ImagePaths
{
    public sealed class ImagePathResponse
    {
        public int Id { get; init; }
        public string Path { get; init; } = default!;
        public int ItemId { get; init; }
        public DateTime? TimeStamp { get; init; }
    }
}
