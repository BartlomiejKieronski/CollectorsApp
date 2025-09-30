using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Contains methods for managing authorization-related data, such as refresh tokens and user retrieval.
    /// </summary>
    public class AuthorizationRepository : IAuthorizationRepository

    {
        private readonly appDatabaseContext _context;
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aesEncryption;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationRepository"/> class with the specified
        /// dependencies.
        /// </summary>
        /// <param name="context">The database context used to interact with the application's data store.</param>
        /// <param name="dataHash">The service used for hashing data, such as passwords or sensitive information.</param>
        /// <param name="aesEncryption">The service used for AES encryption and decryption of sensitive data.</param>
        public AuthorizationRepository(appDatabaseContext context, IDataHash dataHash, IAesEncryption aesEncryption) 
        {
            _context = context;
            _dataHash = dataHash;
            _aesEncryption = aesEncryption;
        }

        /// <summary>
        /// Retrieves a collection of refresh tokens associated with the specified user.
        /// </summary>
        /// <remarks>The returned refresh tokens are retrieved without tracking changes in the database
        /// context.</remarks>
        /// <param name="userId">The unique identifier of the user whose refresh tokens are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of
        /// RefreshTokenInfo objects representing the refresh tokens  for the specified user. If no tokens are found, an
        /// empty collection is returned.</returns>
        public async Task<IEnumerable<RefreshTokenInfo>> GetRefreshTokens(int userId)
        {
            return await _context.RefreshTokens.AsNoTracking().Where(x => userId == x.OwnerId).ToListAsync();
        }

        /// <summary>
        /// Adds a new refresh token to the data store after hashing it.
        /// </summary>
        /// <remarks>The method hashes the provided refresh token before storing it in the data store.
        /// Ensure that the <paramref name="refreshToken"/> object is properly initialized and contains valid
        /// data.</remarks>
        /// <param name="refreshToken">The refresh token information to be added. The <see cref="RefreshTokenInfo.RefreshToken"/> property must
        /// contain the raw token value to be hashed.</param>
        /// <returns>A <see cref="string"/> indicating the result of the operation. Returns "success" if the token is added
        /// successfully.</returns>
        public async Task<string> AddRefreshTokenAsync(RefreshTokenInfo refreshToken)
        {
            var hash = await _dataHash.GenerateHmacAsync(refreshToken.RefreshToken);
            refreshToken.RefreshToken = hash;
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return "succces";
        }

        /// <summary>
        /// Updates the specified refresh token in the data store.
        /// </summary>
        /// <remarks>This method updates the state of the provided refresh token in the underlying data
        /// store.  Ensure that the <paramref name="refreshToken"/> object is valid and corresponds to an existing
        /// entry.</remarks>
        /// <param name="refreshToken">The refresh token information to update. This object must represent an existing token in the data store.</param>
        /// <returns></returns>
        public async Task UpdateRefreshTokenAsync(RefreshTokenInfo refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves information about a refresh token for a specific user.
        /// </summary>
        /// <remarks>This method queries the data context to find the first refresh token that matches the
        /// specified user ID and token string. If no matching token is found, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="userID">The unique identifier of the user associated with the refresh token.</param>
        /// <param name="readRefreshTokenInfo">The refresh token string to look up.</param>
        /// <returns>A <see cref="RefreshTokenInfo"/> object containing details about the refresh token if found; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<RefreshTokenInfo> GetRefreshTokenInfoAsync(int userID, string readRefreshTokenInfo)
        {
            return await _context.RefreshTokens.Where(x => x.OwnerId == userID && x.RefreshToken == readRefreshTokenInfo).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the refresh token information associated with the specified user ID and device ID.
        /// </summary>
        /// <remarks>This method queries the data store to find the first refresh token that matches the
        /// specified user ID and device ID. If no matching token is found, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="userID">The unique identifier of the user who owns the refresh token.</param>
        /// <param name="deviceId">The identifier of the device for which the refresh token was issued.</param>
        /// <returns>A <see cref="RefreshTokenInfo"/> object containing the refresh token details if found; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<RefreshTokenInfo> GetRefreshTokenByDeviceId(int userID, string deviceId)
        {
            return await _context.RefreshTokens.Where(x => x.OwnerId == userID && x.IssuerDeviceInfo == deviceId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves the refresh token information associated with the specified token and issuer device.
        /// </summary>
        /// <remarks>The method hashes the provided refresh token string before attempting to locate a
        /// matching record in the data store. Ensure that the input parameters are valid and non-null before calling
        /// this method.</remarks>
        /// <param name="readRefreshTokenInfo">The raw refresh token string to be validated and matched. This value will be hashed before comparison.</param>
        /// <param name="IssuerDevice">The identifier of the device that issued the refresh token.</param>
        /// <returns>A <see cref="RefreshTokenInfo"/> object representing the matching refresh token, or <see langword="null"/>
        /// if no match is found.</returns>
        public async Task<RefreshTokenInfo> GetRefreshTokenAsync(string readRefreshTokenInfo, string IssuerDevice)
        {
            readRefreshTokenInfo = await _dataHash.GenerateHmacAsync(readRefreshTokenInfo);
            return await _context.RefreshTokens.Where(x => x.RefreshToken == readRefreshTokenInfo && x.IssuerDeviceInfo == IssuerDevice).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a user by their refresh token identifier.
        /// </summary>
        /// <remarks>The user's name and email are decrypted before being returned. If no user is found
        /// with the specified <paramref name="IssuerId"/>, the method returns <see langword="null"/>.</remarks>
        /// <param name="IssuerId">The unique identifier of the user associated with the refresh token.</param>
        /// <returns>A <see cref="Users"/> object representing the user if found; otherwise, <see langword="null"/>.</returns>
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
