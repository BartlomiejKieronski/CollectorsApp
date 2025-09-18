using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class ImagePath : BaseModel
    {
        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public bool IsRemoved { get; set; } = false;
    }
}
