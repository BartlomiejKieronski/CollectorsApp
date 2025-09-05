using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class ImagePathRepository : QueryRepository<ImagePath, ImagePathFilter>, IImagePathRepository
    {
        public ImagePathRepository(appDatabaseContext context) :base(context) 
        {
            
        }

        public async Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId)
        {
            return await _context.ImagePaths.Where(x=>x.ItemId == itemId).ToListAsync();
        }
    }
}
