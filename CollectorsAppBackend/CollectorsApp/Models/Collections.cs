using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class Collections : BaseModel
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public int ParentId { get; set; }
        [MaxLength(200)]
        public string? ParentName { get; set; }
        [Required]
        public int Depth { get; set; }
        [Required]
        public bool IsRemoved { get; set; } = false;
    }
}
