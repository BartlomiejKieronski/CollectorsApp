//using CollectorsApp.Filters;
using CollectorsApp.Models;
using ServiceStack;

namespace CollectorsApp.Repository.Interfaces
{
    public interface ICollectableItemsRepository
    {
        Task<IEnumerable<CollectableItems>> GetCollectableItems();
        Task<CollectableItems> GetCollectableItem(int id);
        Task<CollectableItems> PostCollectableItem(CollectableItems item);
        Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserId(int userId);
        Task<IEnumerable<CollectableItems>> GetCollectableItemsByUserIdAndCollectionId(int userId, int collectionId);
        Task UpdateCollectableItem(CollectableItems item, int Id);
        Task DeleteCollectableItem(int id);
        Task<IEnumerable<CollectableItems>> GetSetAmmountItems(int page, int userId, int collectionId, int numberOfItems);
        Task<CollectableItems> GetCollectableItemByUserId(int item, int userId);
        Task<int> GetItemsCount(int collection, int userId);
        //Task<IEnumerable<CollectableItems>> GetCollectableItemsTest(CollectableItemFilter filter); 
        //Task<PagedResult<CollectableItems>> GetFilteredData();
    }
}
