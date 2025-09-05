using CollectorsApp.Data;
using CollectorsApp.Models;
using Microsoft.EntityFrameworkCore;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Repository
{
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        public UserRepository(appDatabaseContext context, IDataHash dataHash, IAesEncryption aesEncryption) : base(context)
        {
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
        }
        
        
        public async Task<string> PostUser(Users user)
        {

            var dbCheck = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.HashedName== user.HashedName|| x.HashedEmail== user.HashedEmail);

            if (dbCheck != null)
            {
                return "user exists";
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "Created successfully";
        }

        public async Task<Users> GetUserByNameOrEmailAsync(LoginInfo user)
        {
            var hashedData = await _dataHash.GenerateHmacAsync(user.name);
            var data = await _context.Users.AsNoTracking().Where(i => i.HashedName == hashedData || i.HashedEmail == hashedData).FirstOrDefaultAsync();
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
