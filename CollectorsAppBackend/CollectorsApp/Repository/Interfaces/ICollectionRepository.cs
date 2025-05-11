using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface ICollectionRepository
    {
        Task<IEnumerable<Collections>> GetCollections();
        Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId);
        Task PostCollection(Collections colection);
        Task<Collections> GetACollection(int id);
        Task UpdateCollection(Collections collection, int id);
        Task DeleteCollection(int id);
        Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId, string name);
        Task<bool> DetermineChildComponent(int id);
        Task<bool> IsCollectionNameForUserUnique(int userId, string name);
    }
}
