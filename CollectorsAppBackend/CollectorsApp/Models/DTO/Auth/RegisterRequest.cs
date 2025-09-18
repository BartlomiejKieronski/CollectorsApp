using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Auth
{
    public sealed class RegisterRequest
    {
        [Required, MaxLength(1000)]
        public string Name { get; init; } = default!;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; init; } = default!;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; init; } = default!;
    }
}
