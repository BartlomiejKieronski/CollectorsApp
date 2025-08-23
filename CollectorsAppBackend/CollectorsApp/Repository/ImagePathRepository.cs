using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class ImagePathRepository : CRUDImplementation<ImagePath>, IImagePathRepository
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
