using CollectorsApp.Data;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly appDatabaseContext _context;
        public CollectionRepository(appDatabaseContext context)
        {
            _context = context;
        }
        public async Task DeleteCollection(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Remove(collection);
                await _context.SaveChangesAsync();
            }
            else
            {
                return;
            }

        }

        public async Task<Collections?> GetACollection(int id)
        {
            return await _context.Collections.FindAsync(id);
        }

        public async Task<IEnumerable<Collections>> GetCollections()
        {
            return await _context.Collections.ToListAsync();
        }

        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId)
        {

            return await _context.Collections.Where(x => x.OwnerId == userId).ToListAsync();
        }
        public async Task PostCollection(Collections collection)
        {
            await _context.Collections.AddAsync(collection);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCollection(Collections collection, int id)
        {
            _context.Collections.Update(collection);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId,string name)
        {
            return await _context.Collections.Where(x => x.OwnerId == userId && x.Name == name).ToListAsync();
        }
        public async Task<bool> DetermineChildComponent (int id)
        {
            return await _context.Collections.AnyAsync(x=> x.ParentId ==id);
        }

        public async Task<bool> IsCollectionNameForUserUnique(int userId, string name)
        {
            return await _context.Collections.AsNoTracking().AnyAsync(x=>x.OwnerId == userId && x.Name == name);
        }
    }
}
