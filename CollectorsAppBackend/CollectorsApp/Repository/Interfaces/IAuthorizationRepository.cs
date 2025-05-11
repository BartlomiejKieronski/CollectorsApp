using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IAuthorizationRepository
    {

        Task<IEnumerable<RefreshTokenInfo>> GetRefreshTokens(int Id);
        Task<string> AddRefreshTokenAsync(RefreshTokenInfo refreshToken);
        Task UpdateRefreshTokenAsync(RefreshTokenInfo refreshToken);
        Task<RefreshTokenInfo> GetRefreshTokenInfoAsync(int id, string readRefreshTokenInfo);
        Task<RefreshTokenInfo> GetRefreshTokenAsync(string readRefreshTokenInfo, string IssuerDevice);
        Task<Users> GetUserByRefreshTokenAsync(int IssuerId);
        Task<RefreshTokenInfo> GetRefreshTokenByDeviceId(int id, string deviceId);
    }
}
