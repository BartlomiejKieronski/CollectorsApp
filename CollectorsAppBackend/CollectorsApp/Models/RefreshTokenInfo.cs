using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models
{
    public class RefreshTokenInfo : BaseModel
    {
        public string? RefreshToken { get; set; }
        [Required]
        public DateTime DateOfIssue { get; set; }
        public string? IssuerDeviceInfo { get; set; }
        [Required]
        public bool IsValid { get; set; } = false;
    }
}
