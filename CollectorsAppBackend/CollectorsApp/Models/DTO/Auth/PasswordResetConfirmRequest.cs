using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Auth
{
    public sealed class PasswordResetConfirmRequest
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; init; } = default!;
        [Required]
        public string Token { get; init; } = default!;
        [Required, StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; init; } = default!;
    }
}
