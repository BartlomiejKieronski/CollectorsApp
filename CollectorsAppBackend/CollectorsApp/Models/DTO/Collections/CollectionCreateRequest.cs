using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Collections
{
    public sealed class CollectionCreateRequest
    {
        [Required, MaxLength(150)]
        public string Name { get; init; } = default!;
        [Required] 
        public int ParentId { get; init; }
        [MaxLength(200)] 
        public string? ParentName { get; init; }
        [Required]
        public int Depth { get; init; }
        public int OwnerId { get; set; }
    }
}
