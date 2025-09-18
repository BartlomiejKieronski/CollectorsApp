namespace CollectorsApp.Models.DTO.Users
{
    public sealed class UserResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Role { get; init; } = default!;
        public bool Active { get; init; }
        public DateTime? LastLogin { get; init; }
        public DateTime? LastLogout { get; init; }
        public DateTime? LastUpdated { get; init; }
        public bool IsSusspended { get; init; }
        public bool IsBanned { get; init; }
        public DateTime? AccountCreationDate { get; init; }
    }
}
