using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Creates a repository for managing collectable items with various query methods.
    /// </summary>
    public class CollectableItemRepository : QueryRepository<CollectableItems, CollectableItemsFilter>, ICollectableItemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectableItemRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <remarks>This repository provides data access functionality for collectable items, leveraging
        /// the provided database context. Ensure that the <paramref name="context"/> is properly configured and
        /// disposed of when no longer needed.</remarks>
        /// <param name="context">The database context used to interact with the underlying data store. Cannot be <see langword="null"/>.</param>
        public CollectableItemRepository(appDatabaseContext context) : base(context) {
            
        }

        /// <summary>
        /// Retrieves a collection of items owned by the specified user.
        /// </summary>
        /// <remarks>This method queries the database for items where the owner matches the specified user
        /// ID.  Ensure that the user ID provided is valid and corresponds to an existing user.</remarks>
        /// <param name="userId">The unique identifier of the user whose collectable items are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> 
        /// of <see cref="CollectableItems"/> representing the items owned by the specified user.  If the user has no
        /// items, the collection will be empty.</returns>
        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserId(int userId)
        {
            return await _context.CollectableItems.Where(x => x.OwnerId == userId).ToListAsync();
        }

        /// <summary>
        /// Retrieves a collection of collectable items that belong to a specific user and are part of a specified
        /// collection.
        /// </summary>
        /// <remarks>This method queries the database asynchronously to retrieve the matching collectable
        /// items.  Ensure that the provided user ID and collection ID are valid and correspond to existing
        /// entities.</remarks>
        /// <param name="userId">The unique identifier of the user who owns the collectable items.</param>
        /// <param name="collectionId">The unique identifier of the collection to which the collectable items belong.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> 
        /// of <see cref="CollectableItems"/> representing the items owned by the specified user and associated with the
        /// specified collection. If no items match the criteria, the result will be an empty collection.</returns>
        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdAndCollectionId(int userId, int collectionId)
        {
            return await _context.CollectableItems.Where(x=>x.OwnerId == userId && x.CollectionId==collectionId).ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of collectable items for a specific user and collection.
        /// </summary>
        /// <remarks>This method uses zero-based indexing for pagination. For example, if <paramref
        /// name="page"/> is 1,  the first <paramref name="numberOfItems"/> items will be returned. Ensure that the
        /// combination of  <paramref name="page"/> and <paramref name="numberOfItems"/> does not exceed the total
        /// number of items  available in the specified collection.</remarks>
        /// <param name="page">The page number to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="userId">The unique identifier of the user who owns the items.</param>
        /// <param name="collectionId">The unique identifier of the collection to filter items by.</param>
        /// <param name="numberOfItems">The number of items to include in each page. Must be greater than 0.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of 
        /// <see cref="CollectableItems"/> representing the items in the specified page.</returns>
        public async Task<IEnumerable<CollectableItems>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems)
        {
            return await _context.CollectableItems
                .Where(x=> x.CollectionId==collectionId && x.OwnerId == userId)
                .Skip((page - 1) * numberOfItems)
                .Take(numberOfItems)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a collectable item associated with the specified item ID and user ID.
        /// </summary>
        /// <remarks>This method performs an asynchronous query to retrieve the collectable item that
        /// matches the specified item ID and user ID. If no matching item is found, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="item">The unique identifier of the collectable item to retrieve.</param>
        /// <param name="userId">The unique identifier of the user who owns the collectable item.</param>
        /// <returns>A <see cref="CollectableItems"/> object representing the collectable item if found; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<CollectableItems> GetCollectableItemByUserId(int item,int userId)
        {
            return await _context.CollectableItems.Where(x => x.Id == item && x.OwnerId == userId).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Asynchronously retrieves the total number of items in a specified collection owned by a specific user.
        /// </summary>
        /// <param name="collection">The identifier of the collection to count items from.</param>
        /// <param name="userId">The identifier of the user who owns the items in the collection.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total number of items  in
        /// the specified collection owned by the specified user.</returns>
        public async Task<int> GetItemsCount(int collection, int userId)
        {
            return await _context.CollectableItems.CountAsync(x => x.CollectionId == collection && x.OwnerId == userId);
        }
    }
}
