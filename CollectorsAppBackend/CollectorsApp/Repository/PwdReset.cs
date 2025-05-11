using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class PwdReset : IPwdReset
    {
        private readonly appDatabaseContext _context;
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aes;
        
        public PwdReset(appDatabaseContext context, IDataHash dataHash, IAesEncryption aes) {
            _context = context;
            _dataHash = dataHash;
            _aes = aes;
        }

        public async Task DeletePasswordReset(int id)
        {
            var passwordReset= await _context.PwdReset.FindAsync(id);
            if (passwordReset != null)
            {
                _context.Remove(passwordReset);
                await _context.SaveChangesAsync();
            }
            else
            {
                return;
            }
        }

        public async Task PostPasswordReset(PasswordResetModel model)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(model.Email);
            var tokenHash = await _dataHash.GenerateHmacAsync(model.Token);
            model.Email = emailHash;
            model.Token = tokenHash;
            await _context.PwdReset.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task PutPasswordReset(PasswordResetModel model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetModel> GetPasswordResetModelByEmail(string email)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(email);

            return await _context.PwdReset.AsNoTracking().FirstOrDefaultAsync(x => x.Email == emailHash);
        }
    }
}