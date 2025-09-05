//using CollectorsApp.Filters;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface ICollectableItemRepository : IQueryRepository<CollectableItems, CollectableItemsFilter>, IGenericRepository<CollectableItems>
    {
        
        Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserId(int userId);
        Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdAndCollectionId(int userId, int collectionId);
        Task<IEnumerable<CollectableItems>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems);
        Task<CollectableItems> GetCollectableItemByUserId(int item, int userId);
        Task<int> GetItemsCount(int collection, int userId);
        //Task<IEnumerable<CollectableItems>> GetCollectableItemsTest(CollectableItemFilter filter); 
        //Task<PagedResult<CollectableItems>> GetFilteredData();
    }
}
