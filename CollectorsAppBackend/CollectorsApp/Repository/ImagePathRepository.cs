using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class ImagePathRepository : IImagePathRepository
    {
        private readonly appDatabaseContext _context;
        public ImagePathRepository(appDatabaseContext context) {
            _context = context;
        }

        public async Task DeleteImagePath(int id)
        {
            var imagePath = await _context.ImagePaths.FindAsync(id);
            if (imagePath != null) { 
                _context.Remove(imagePath);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ImagePath> GetImagePathByImageId(int id)
        {
            return await _context.ImagePaths.FindAsync(id);
        }
        public async Task PostImagePath(ImagePath imagePath)
        {
            await _context.ImagePaths.AddAsync(imagePath);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ImagePath>> GetImagePaths()
        {
            return await _context.ImagePaths.ToListAsync();
        }

        public async Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId)
        {
            return await _context.ImagePaths.Where(x=>x.ItemId == itemId).ToListAsync();
        }

        public async Task UpdateImagePath(ImagePath imagePath, int id)
        {
            _context.Entry(imagePath).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }   
    }
}
