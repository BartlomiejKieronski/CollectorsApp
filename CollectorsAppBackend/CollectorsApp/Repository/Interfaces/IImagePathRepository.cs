using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IImagePathRepository :IGenericRepository<ImagePath>
    {
        
        Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId);
        

    }
}
