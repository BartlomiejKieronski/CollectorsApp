using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IImagePathRepository :ICRUD<ImagePath>
    {
        
        Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId);
        

    }
}
