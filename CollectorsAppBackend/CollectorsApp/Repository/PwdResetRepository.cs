using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using CollectorsApp.Services.Encryption;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides data access functionality for managing <see cref="PasswordReset"/> entities,  including operations for
    /// creating, retrieving, and deleting password reset records.
    /// </summary>
    /// <remarks>This repository extends the functionality of <see cref="GenericRepository{T}"/> to include 
    /// additional operations specific to password reset records, such as hashing sensitive data  (e.g., email and
    /// token) before storage and retrieving records by hashed email.</remarks>
    public class PwdResetRepository : GenericRepository<PasswordReset> ,IPwdResetRepository
    {
        private readonly IDataHash _dataHash;
        private readonly IAesEncryption _aes;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PwdResetRepository"/> class with the specified database
        /// context, data hashing service, and AES encryption service.
        /// </summary>
        /// <param name="context">The database context used to interact with the application's data store.</param>
        /// <param name="dataHash">The service used for hashing data, such as passwords or tokens.</param>
        /// <param name="aes">The AES encryption service used for encrypting and decrypting sensitive data.</param>
        public PwdResetRepository(appDatabaseContext context, IDataHash dataHash, IAesEncryption aes) : base(context) {
            _dataHash = dataHash;
            _aes = aes;
        }

        /// <summary>
        /// Deletes the entity with the specified identifier asynchronously.
        /// </summary>
        /// <remarks>If the entity with the specified identifier does not exist, the method returns <see
        /// langword="false"/>  without performing any deletion.</remarks>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Asynchronously adds a new password reset entity to the database after hashing its email and token.
        /// </summary>
        /// <remarks>This method hashes the email and token values of the provided <see
        /// cref="PasswordReset"/> entity using HMAC before persisting the entity to the database. The original values
        /// of the email and token are replaced with their hashed equivalents.</remarks>
        /// <param name="entity">The <see cref="PasswordReset"/> entity to be added. The <see cref="PasswordReset.Email"/> and <see
        /// cref="PasswordReset.Token"/> properties must be set prior to calling this method.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task PostAsync(PasswordReset entity)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(entity.Email);
            var tokenHash = await _dataHash.GenerateHmacAsync(entity.Token);
            entity.Email = emailHash;
            entity.Token = tokenHash;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves the password reset model associated with the specified email address.
        /// </summary>
        /// <remarks>The email address is hashed before querying the database. Ensure that the provided
        /// email matches the format expected by the hashing mechanism.</remarks>
        /// <param name="email">The email address of the user requesting a password reset. This value must not be null or empty.</param>
        /// <returns>A <see cref="PasswordReset"/> object representing the password reset model if a match is found; otherwise,
        /// <see langword="null"/>.</returns>
        public async Task<PasswordReset> GetPasswordResetModelByEmail(string email)
        {
            var emailHash = await _dataHash.GenerateHmacAsync(email);

            return await _context.PwdReset.AsNoTracking().FirstOrDefaultAsync(x => x.Email == emailHash);
        }
    }
}