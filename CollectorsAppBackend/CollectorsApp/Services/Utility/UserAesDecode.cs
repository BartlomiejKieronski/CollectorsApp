using CollectorsApp.Models;
using CollectorsApp.Models.DTO.Auth;
using CollectorsApp.Services.Encryption;

namespace CollectorsApp.Services.Utility
{
    /// <summary>
    /// Provides functionality to decrypt user data using AES decryption.
    /// </summary>
    /// <remarks>This class is responsible for decrypting sensitive fields, such as Name and Email, in
    /// user-related objects. It relies on an implementation of <see cref="IAesEncryption"/> to perform the
    /// decryption.</remarks>
    public class UserAesDecode : IUserAesDecode
    {
        private readonly IAesEncryption _aesEncryption;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAesDecode"/> class with the specified AES encryption
        /// service.
        /// </summary>
        /// <param name="aesEncryption">The AES encryption service used to perform decryption operations.</param>
        public UserAesDecode(IAesEncryption aesEncryption)
        {
            _aesEncryption = aesEncryption;
        }

        /// <summary>
        /// Decrypts the encrypted user data and returns the user object with decrypted values.
        /// </summary>
        /// <remarks>This method uses AES encryption to decrypt the <see cref="Users.Name"/> and <see
        /// cref="Users.Email"/> properties of the provided <see cref="Users"/> object. Ensure that the initialization
        /// vector keys (<see cref="Users.NameIVKey"/> and <see cref="Users.EmailIVKey"/>) are correctly set before
        /// calling this method.</remarks>
        /// <param name="hasheduser">A <see cref="Users"/> object containing encrypted user data. The <see cref="Users.Name"/> and <see
        /// cref="Users.Email"/> properties must be encrypted, and their corresponding initialization vector keys (<see
        /// cref="Users.NameIVKey"/> and <see cref="Users.EmailIVKey"/>) must be provided.</param>
        /// <returns>A <see cref="Users"/> object with the <see cref="Users.Name"/> and <see cref="Users.Email"/> properties
        /// decrypted.</returns>
        public async Task<Users> GetUserDataFromEncryption(Users hasheduser)
        {
            hasheduser.Name = await _aesEncryption.AesDecrypt(hasheduser.Name, hasheduser.NameIVKey);
            hasheduser.Email = await _aesEncryption.AesDecrypt(hasheduser.Email, hasheduser.EmailIVKey);
            return hasheduser;
        }

        /// <summary>
        /// Decrypts the user data for a logged-in user.
        /// </summary>
        /// <remarks>This method decrypts the name and email fields of the provided <paramref
        /// name="hasheduser"/> using AES encryption. The role and ID fields are directly copied without
        /// modification.</remarks>
        /// <param name="hasheduser">The user object containing encrypted data and associated encryption keys.</param>
        /// <returns>A <see cref="LoggedUserInfo"/> object containing the decrypted user data, including the user's name and
        /// email.</returns>
        public async Task<LoggedUserInfo> LoggedUserDataDecrypt(Users hasheduser)
        {
            LoggedUserInfo user = new LoggedUserInfo
            {
                Id = hasheduser.Id,
                Name = await _aesEncryption.AesDecrypt(hasheduser.Name, hasheduser.NameIVKey),
                Email = await _aesEncryption.AesDecrypt(hasheduser.Email, hasheduser.EmailIVKey),
                Role = hasheduser.Role
            };
            return user;
        }
    }
}
