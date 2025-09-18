using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class UserPreferences : BaseModel
    {
        [Required]
        [MaxLength(50)]
        public string Layout { get; set; } 
        [Required]
        [MaxLength(50)]
        public string Theme { get; set; } 
        [Required]
        public int ItemsPerPage { get; set; } 
        [Required]
        public bool Pagination { get; set; } = true;
    }
}
