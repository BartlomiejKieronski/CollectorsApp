using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.ImagePaths
{
    public sealed class ImagePathCreateRequest
    {
        [Required, MaxLength(500)]
        public string Path { get; init; } = default!;
        [Required] 
        public int ItemId { get; init; }
    }
}
