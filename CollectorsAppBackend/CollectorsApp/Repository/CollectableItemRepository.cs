using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class CollectableItemRepository : QueryRepository<CollectableItems, CollectableItemsFilter>, ICollectableItemRepository
    {
        public CollectableItemRepository(appDatabaseContext context) : base(context) {
            
        }

        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserId(int userId)
        {
            return await _context.CollectableItems.Where(x => x.OwnerId == userId).ToListAsync();
        }

        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdAndCollectionId(int userId, int collectionId)
        {
            return await _context.CollectableItems.Where(x=>x.OwnerId == userId && x.CollectionId==collectionId).ToListAsync();
        }
        public async Task<IEnumerable<CollectableItems>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems)
        {
            return await _context.CollectableItems
                .Where(x=> x.CollectionId==collectionId && x.OwnerId == userId)
                .Skip((page - 1) * numberOfItems)
                .Take(numberOfItems)
                .ToListAsync();
        }
        public async Task<CollectableItems?> GetCollectableItemByUserId(int item,int userId)
        {
            return await _context.CollectableItems.Where(x => x.Id == item && x.OwnerId == userId).FirstOrDefaultAsync();
        }
        public async Task<int> GetItemsCount(int collection, int userId)
        {
            return await _context.CollectableItems.CountAsync(x => x.CollectionId == collection && x.OwnerId == userId);
        }
    }
}
