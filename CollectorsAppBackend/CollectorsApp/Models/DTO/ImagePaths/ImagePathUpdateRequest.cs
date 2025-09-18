namespace CollectorsApp.Models.DTO.ImagePaths
{
    public class ImagePathUpdateRequest
    {
        public int Id { get; init; }
        public string? Path { get; init; }
        public int ItemId { get; init; }
        public bool? IsRemoved { get; init; }
    }
}
