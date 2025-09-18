using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Auth
{
    public sealed class PasswordResetRequest
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; init; } = default!;
    }
}
