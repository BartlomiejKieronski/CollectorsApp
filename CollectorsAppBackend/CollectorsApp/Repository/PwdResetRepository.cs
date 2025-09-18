using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class PwdResetRepository : GenericRepository<PasswordReset> ,IPwdResetRepository
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aes;
        
        public PwdResetRepository(appDatabaseContext context, IDataHash dataHash, IAesEncryption aes) : base(context) {
            _dataHash = dataHash;
            _aes = aes;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var passwordReset = await _dbSet.FindAsync(id);
            if (passwordReset != null)
            {
                _context.Remove(passwordReset);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public override async Task PostAsync(PasswordReset entity)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(entity.Email);
            var tokenHash = await _dataHash.GenerateHmacAsync(entity.Token);
            entity.Email = emailHash;
            entity.Token = tokenHash;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordReset> GetPasswordResetModelByEmail(string email)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(email);

            return await _context.PwdReset.AsNoTracking().FirstOrDefaultAsync(x => x.Email == emailHash);
        }
    }
}