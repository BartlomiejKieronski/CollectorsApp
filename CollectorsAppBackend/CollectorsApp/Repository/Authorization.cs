using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class Authorization : IAuthorizationRepository

    {
        private readonly appDatabaseContext _context;
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        private readonly ILogger<Authorization> _logger;
        public Authorization(appDatabaseContext context, IDataHash dataHash, IAesEncryption aesEncryption, ILogger<Authorization> loger) 
        {
            _context = context;
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
            _logger = loger;
        }
        
        public async Task<IEnumerable<RefreshTokenInfo>> GetRefreshTokens(int Id)
        {
            return await _context.RefreshTokens.AsNoTracking().Where(x => Id == x.OwnerId).ToListAsync();
        }
        public async Task<string> AddRefreshTokenAsync(RefreshTokenInfo refreshToken)
        {
            var hash = await _dataHash.GenerateHmacAsync(refreshToken.RefreshToken);
            refreshToken.RefreshToken = hash;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return "succces";
        }
        public async Task UpdateRefreshTokenAsync(RefreshTokenInfo refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshTokenInfo> GetRefreshTokenInfoAsync(int id, string readRefreshTokenInfo)
        {
            return await _context.RefreshTokens.Where(x => x.OwnerId == id && x.RefreshToken == readRefreshTokenInfo).FirstOrDefaultAsync();
        }
        public async Task<RefreshTokenInfo> GetRefreshTokenByDeviceId(int id, string deviceId)
        {
            return await _context.RefreshTokens.Where(x => x.OwnerId == id && x.IssuerDeviceInfo == deviceId).FirstOrDefaultAsync();
        }
        public async Task<RefreshTokenInfo> GetRefreshTokenAsync(string readRefreshTokenInfo, string IssuerDevice)
        {
            readRefreshTokenInfo = await _dataHash.GenerateHmacAsync(readRefreshTokenInfo);
            return await _context.RefreshTokens.Where(x => x.RefreshToken == readRefreshTokenInfo && x.IssuerDeviceInfo == IssuerDevice).FirstOrDefaultAsync();
        }
        public async Task<Users> GetUserByRefreshTokenAsync(int IssuerId)
        {
            var data = await _context.Users.AsNoTracking().Where(i => i.Id==IssuerId).FirstOrDefaultAsync();

            if (data != null)
            {
                data.Name = await _aesEncryption.AesDecrypt(data.Name, data.NameIVKey);
                data.Email = await _aesEncryption.AesDecrypt(data.Email, data.EmailIVKey);
                return data;
            }
            return null;
        }
    }
}
