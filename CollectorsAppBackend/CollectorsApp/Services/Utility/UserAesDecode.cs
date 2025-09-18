using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Services.Utility
{
    public class UserAesDecode : IUserAesDecode
    {
        private readonly IAesEncryption _aesEncryption;
        public UserAesDecode(IAesEncryption aesEncryption)
        {
            _aesEncryption = aesEncryption;
        }

        public async Task<Users> GetUserDataFromEncryption(Users hasheduser)
        {
            hasheduser.Name = await _aesEncryption.AesDecrypt(hasheduser.Name, hasheduser.NameIVKey);
            hasheduser.Email = await _aesEncryption.AesDecrypt(hasheduser.Email, hasheduser.EmailIVKey);
            return hasheduser;
        }

        public async Task<LoggedUserInfo> LoggedUserDataDecrypt(Users hasheduser)
        {
            LoggedUserInfo user = new LoggedUserInfo();
            
            user.Id = hasheduser.Id;
            user.Name = await _aesEncryption.AesDecrypt(hasheduser.Name, hasheduser.NameIVKey);
            user.Email = await _aesEncryption.AesDecrypt(hasheduser.Email, hasheduser.EmailIVKey);
            user.Role = hasheduser.Role;
            return user;
        }
    }
}
