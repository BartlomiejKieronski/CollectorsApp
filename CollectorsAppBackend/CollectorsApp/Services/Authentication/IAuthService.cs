using CollectorsApp.Models;
using CollectorsApp.Models.AuthResults;

namespace CollectorsApp.Services.Authentication
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginInfo loginInfo, string deviceId);
        Task<ReauthResult> ReauthenticateAsync(string refreshToken, string deviceInfo);
        Task LogoutAsync(int userId, string refreshToken, string deviceInfo);
    }
}
