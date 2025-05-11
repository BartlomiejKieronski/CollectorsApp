using CollectorsApp.Data;
//using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class CollectableItemsRepository : ICollectableItemsRepository
    {
        private readonly appDatabaseContext _context;
        public CollectableItemsRepository(appDatabaseContext context) {
            _context = context;
        }
        public async Task DeleteCollectableItem(int id)
        {
            var data = await _context.CollectableItems.FindAsync(id);
            if(data != null) {
                _context.CollectableItems.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CollectableItems> GetCollectableItem(int id)
        {
            return await _context.CollectableItems.FindAsync(id);
            
        }

        public async Task<IEnumerable<CollectableItems>> GetCollectableItems()
        {
            return await _context.CollectableItems.ToListAsync();
        }

        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserId(int userId)
        {
            return await _context.CollectableItems.Where(x => x.OwnerId == userId).ToListAsync();
        }

        public async Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdAndCollectionId(int userId, int collectionId)
        {
            return await _context.CollectableItems.Where(x=>x.OwnerId == userId && x.CollectionId==collectionId).ToListAsync();
        }
        public async Task<CollectableItems> PostCollectableItem(CollectableItems item)
        {
            item.InsertDate = DateTime.Now;
            await _context.CollectableItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }
        public async Task UpdateCollectableItem(CollectableItems item, int Id)
        {
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CollectableItems>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems)
        {
            return await _context.CollectableItems
                .Where(x=> x.CollectionId==collectionId && x.OwnerId == userId)
                .Skip((page - 1) * numberOfItems)
                .Take(numberOfItems)
                .ToListAsync();
        }
        public async Task<CollectableItems> GetCollectableItemByUserId(int item,int userId)
        {
            return await _context.CollectableItems.Where(x => x.Id == item && x.OwnerId == userId).FirstOrDefaultAsync();
        }
        public async Task<int> GetItemsCount(int collection, int userId)
        {
            return await _context.CollectableItems.CountAsync(x => x.CollectionId == collection && x.OwnerId == userId);
        }
        /*public async Task<IEnumerable<CollectableItems>> GetCollectableItemsTest(CollectableItemFilter filter)
        {
            IQueryable<CollectableItems> data = _context.CollectableItems;
            if (filter.singleItem.HasValue)
            {
                return (IEnumerable<CollectableItems>)await data.Where(x => x.Id == filter.singleItem).FirstOrDefaultAsync();
            }
            
            if(filter.collection.HasValue)
            {
                data = data.Where(x => x.CollectionId == filter.collection.Value);
            }

            data = (IQueryable<CollectableItems>)await data.ToListAsync();
            return data;
        }*/
    }
}
