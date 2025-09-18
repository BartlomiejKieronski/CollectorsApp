using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Collections
{
    public sealed class CollectionUpdateRequest
    {
        [MaxLength(150)] 
        public string? Name { get; init; }
        public int? ParentId { get; init; }
        [MaxLength(200)]
        public string? ParentName { get; init; }
        public int? Depth { get; init; }
        public bool? IsRemoved { get; init; }
    }
}
