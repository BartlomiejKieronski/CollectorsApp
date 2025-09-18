using CollectorsApp.Models.AuthResults;
using CollectorsApp.Models.DTO.Auth;

namespace CollectorsApp.Services.Authentication
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest loginInfo, string deviceId);
        Task<ReauthResult> ReauthenticateAsync(string refreshToken, string deviceInfo);
        Task LogoutAsync(int userId, string refreshToken, string deviceInfo);
    }
}
