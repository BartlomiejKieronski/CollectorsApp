using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class UserConsent : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string ConsentType { get; set; }
        [Required]
        public bool IsGranted { get; set; } = false;
    }
}
