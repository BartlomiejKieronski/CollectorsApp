namespace CollectorsApp.Models.DTO.UserPreferences
{
    public sealed class UserPreferencesResponse
    {
        public int Id { get; init; }
        public string Layout { get; init; } = default!;
        public string Theme { get; init; } = default!;
        public int? ItemsPerPage { get; init; }
        public bool? Pagination { get; init; }
        public DateTime? LastUpdated { get; init; }
    }
}
