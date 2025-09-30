using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides methods for querying and managing collections in the application database.
    /// </summary>
    /// <remarks>The <see cref="CollectionRepository"/> class is responsible for retrieving, filtering, and
    /// validating collections based on user-specific criteria. It extends the <see cref="QueryRepository{TEntity,
    /// TFilter}"/> base class to provide additional functionality specific to collections, such as determining child
    /// components and ensuring collection name uniqueness.</remarks>
    public class CollectionRepository : QueryRepository<Collections, CollectionFilters>, ICollectionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <remarks>This constructor sets up the repository to operate on collections within the provided
        /// database context. Ensure that the <paramref name="context"/> is properly configured and disposed of when no
        /// longer needed.</remarks>
        /// <param name="context">The database context used to interact with the underlying data store. Cannot be <see langword="null"/>.</param>
        public CollectionRepository(appDatabaseContext context) : base(context)
        {
            
        }

        /// <summary>
        /// Retrieves a collection of items owned by the specified user.
        /// </summary>
        /// <remarks>This method queries the database asynchronously to retrieve all collections where the
        /// owner matches the specified user ID. Ensure that the database context is properly configured and accessible
        /// before calling this method.</remarks>
        /// <param name="userId">The unique identifier of the user whose collections are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of <see cref="Collections"/> representing the collections owned by the specified user. If the user has no
        /// collections, the result will be an empty enumerable.</returns>
        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId)
        {

            return await _context.Collections.Where(x => x.OwnerId == userId).ToListAsync();
        }

        /// <summary>
        /// Retrieves a collection of <see cref="Collections"/> objects that belong to the specified user and match the
        /// given name.
        /// </summary>
        /// <remarks>This method performs a case-sensitive search for collections based on the provided
        /// name. Ensure that the <paramref name="name"/> parameter matches the exact case of the collection names
        /// stored in the database.</remarks>
        /// <param name="userId">The unique identifier of the user whose collections are to be retrieved.</param>
        /// <param name="name">The name of the collections to filter by. Only collections with this name will be included in the result.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of <see cref="Collections"/> that belong to the specified user and match the given name. If no matching
        /// collections are found, the result will be an empty enumerable.</returns>
        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId,string name)
        {
            return await _context.Collections.Where(x => x.OwnerId == userId && x.Name == name).ToListAsync();
        }

        /// <summary>
        /// Determines whether a child component exists for the specified parent identifier.
        /// </summary>
        /// <param name="id">The identifier of the parent component to check for child components.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/>  if a child
        /// component exists for the specified parent identifier; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DetermineChildComponent (int id)
        {
            return await _context.Collections.AnyAsync(x=> x.ParentId ==id);
        }

        /// <summary>
        /// Determines whether a collection name is unique for a specific user.
        /// </summary>
        /// <remarks>This method performs a case-sensitive check to determine if the collection name  is
        /// already associated with the specified user.</remarks>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="name">The name of the collection to check for uniqueness.</param>
        /// <returns><see langword="true"/> if the specified collection name already exists for the user;  otherwise, <see
        /// langword="false"/>.</returns>
        public async Task<bool> IsCollectionNameForUserUnique(int userId, string name)
        {
            return await _context.Collections.AsNoTracking().AnyAsync(x=>x.OwnerId == userId && x.Name == name);
        }
    }
}
