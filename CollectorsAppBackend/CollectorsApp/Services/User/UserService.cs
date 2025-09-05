using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using ServiceStack;

namespace CollectorsApp.Services.User
{
    public class UserService : IUserService
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        private readonly IUserRepository _userRepository;
        public UserService(IDataHash dataHash, IAesEncryption aesEncryption, IUserRepository userRepository) 
        {
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
            _userRepository = userRepository;
        }

        public async Task<string> RegisterUserAsync(Users user)
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
            return await _userRepository.PostUser(user);
        }
    }
}
