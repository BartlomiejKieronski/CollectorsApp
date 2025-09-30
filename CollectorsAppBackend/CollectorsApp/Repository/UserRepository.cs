using CollectorsApp.Data;
using CollectorsApp.Models;
using Microsoft.EntityFrameworkCore;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using CollectorsApp.Models.DTO.Auth;

namespace CollectorsApp.Repository
{

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class
    /// with the specified database context, data hashing service, and AES encryption service.
    /// </summary>
    /// <param name="context">The database context used for accessing and managing user data.</param>
    /// <param name="dataHash">The service used for hashing sensitive data such as passwords.</param>
    /// <param name="aesEncryption">The service used for AES encryption and decryption of user data.</param>
    public class UserRepository : GenericRepository<Users>, IUserRepository
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class with the specified database context,
        /// data hashing service, and AES encryption service.
        /// </summary>
        /// <param name="context">The database context used to interact with the application's data store.</param>
        /// <param name="dataHash">The service used for hashing sensitive user data.</param>
        /// <param name="aesEncryption">The service used for encrypting and decrypting user data.</param>
        public UserRepository(appDatabaseContext context, IDataHash dataHash, IAesEncryption aesEncryption) : base(context)
        {
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
        }

        /// <summary>
        /// Adds a new user to the database if a user with the same hashed name or hashed email does not already exist.
        /// </summary>
        /// <remarks>This method performs a case-insensitive check for existing users based on the hashed
        /// name and hashed email. Ensure that the <paramref name="user"/> parameter is properly populated before
        /// calling this method.</remarks>
        /// <param name="user">The user entity to be added. The <see cref="Users.HashedName"/> and <see cref="Users.HashedEmail"/>
        /// properties must be unique.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The task result is: <list
        /// type="bullet"> <item><description>A string containing the ID of the newly added user if the operation is
        /// successful.</description></item> <item><description>"user exists" if a user with the same hashed name or
        /// hashed email already exists in the database.</description></item> </list></returns>
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

            return user.Id.ToString();
        }

        /// <summary>
        /// Retrieves a user by their hashed name or hashed email address.
        /// </summary>
        /// <remarks>The method searches for a user whose hashed name or hashed email matches the provided
        /// input. If a matching user is found, their name and email are decrypted before being returned.</remarks>
        /// <param name="user">The login request containing the user's name or email to search for.</param>
        /// <returns>A <see cref="Users"/> object representing the user if found; otherwise, <see langword="null"/>.</returns>
        public async Task<Users> GetUserByNameOrEmailAsync(LoginRequest user)
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
