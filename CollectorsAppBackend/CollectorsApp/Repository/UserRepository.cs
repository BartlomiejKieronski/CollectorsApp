using CollectorsApp.Data;
using CollectorsApp.Models;
using Microsoft.EntityFrameworkCore;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Repository
{
    public class UserRepository : CRUDImplementation<Users>, IUserRepository
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
            var credentials = await _dataHash.GetCredentialsAsync(user.Password);
            var hashName = await _dataHash.GenerateHmacAsync(user.Name);
            var hashEmail = await _dataHash.GenerateHmacAsync(user.Email);
            var encryptedName = await _aesEncryption.AesEncrypt(user.Name);
            var encryptedEmail = await _aesEncryption.AesEncrypt(user.Email);   

            user.Email = encryptedEmail.Item1;
            user.EmailIVKey = encryptedEmail.Item2;
            user.HashedEmail = hashEmail;
            user.Name = encryptedName.Item1;
            user.NameIVKey = encryptedName.Item2;
            user.HashedName = hashName;
            user.Salt = credentials.Item1;
            user.Password = credentials.Item2;
            
            user.Active = true;
            user.Role = "user";
            user.AccountCreationDate = DateTime.Now;
            user.IsBanned = false;
            user.IsSusspended = false;
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
