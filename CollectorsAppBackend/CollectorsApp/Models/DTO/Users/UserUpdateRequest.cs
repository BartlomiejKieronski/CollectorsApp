using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Users
{
    // For partial updates, all fields optional
    public sealed class UserUpdateRequest
    {
        [MaxLength(100)]
        public string? Name { get; init; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; init; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; init; }

        // Only expose if you intend to let admins change these
        public bool? Active { get; init; }
        [MaxLength(32)]
        public string? Role { get; init; }
        public bool? IsSusspended { get; init; }
        public bool? IsBanned { get; init; }
    }
}
