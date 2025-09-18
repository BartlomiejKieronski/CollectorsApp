using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.UserPreferences
{
    public sealed class UserPreferencesUpdateRequest
    {
        [MaxLength(50)] 
        public string? Layout { get; init; }
        [MaxLength(50)] 
        public string? Theme { get; init; }
        [Range(1, 500)] 
        public int? ItemsPerPage { get; init; }
        public bool? Pagination { get; init; }
    }
}
