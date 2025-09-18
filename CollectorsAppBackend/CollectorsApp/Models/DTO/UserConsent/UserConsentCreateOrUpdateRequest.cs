using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.UserConsent
{
    public sealed class UserConsentCreateOrUpdateRequest
    {
        [Required, MaxLength(100)] 
        public string ConsentType { get; init; } = default!;
        [Required] 
        public bool IsGranted { get; init; }
    }
}
