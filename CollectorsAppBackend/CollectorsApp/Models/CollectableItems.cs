using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class CollectableItems : BaseModel
    {
        [Required]
        [MaxLength(200)]
        public string ItemName { get; set; }
        public DateTime? ItemYear { get; set; }
        [MaxLength(100)]
        public string? ItemNumismat { get; set; }
        [MaxLength(100)]
        public string? ItemValue { get; set; }
        [MaxLength(500)]
        public string? PhotoFilePath { get; set; }
        public DateTime? DateOfAquire { get; set; }
        [Required]
        public int CollectionId { get; set; }
        [MaxLength(50)]
        public string? State { get; set; }
        public string? Description {  get; set; }
        [Required]
        public bool IsRemoved { get; set; } = false;
    }
}
