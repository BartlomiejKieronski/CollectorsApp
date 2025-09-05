using CollectorsApp.Filters;
using CollectorsApp.Models;

namespace CollectorsApp.Repository.Interfaces
{
    public interface IImagePathRepository : IQueryRepository<ImagePath,ImagePathFilter>, IGenericRepository<ImagePath>
    {
        
        Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId);
        

    }
}
