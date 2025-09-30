using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides data access functionality for managing user consents in the application.
    /// </summary>
    /// <remarks>This repository is responsible for querying and managing <see cref="UserConsent"/> entities, 
    /// including filtering and retrieving user consents based on specific criteria. It extends  <see
    /// cref="QueryRepository{TEntity, TFilter}"/> to provide additional functionality specific  to user
    /// consents.</remarks>
    public class UserConsentRepository : QueryRepository<UserConsent,UserConsentFilter>, IUserConsentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserConsentRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <param name="context">The database context used to interact with the application's data store. Cannot be null.</param>
        public UserConsentRepository(appDatabaseContext context) : base(context)
        {
            

        }

        /// <summary>
        /// Retrieves a collection of user consents associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose consents are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of
        /// UserConsent objects associated with the specified user ID.  If no consents are found, an empty collection is
        /// returned.</returns>
        public async Task<IEnumerable<UserConsent>> GetByUserIdAsync(int userId)
        {
            return await _context.UserConsents.Where(x => x.OwnerId == userId).ToListAsync();
        }


    }
}
