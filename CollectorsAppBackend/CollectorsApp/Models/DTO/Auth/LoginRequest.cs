using System.ComponentModel.DataAnnotations;

namespace CollectorsApp.Models.DTO.Auth
{
    public class LoginRequest
    {
        [Required, MaxLength(100)]
        public string name { get; init; } = default!;

        [Required, StringLength(100, MinimumLength = 6)]
        public string password { get; init; } = default!;
    }
}
