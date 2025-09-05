using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class CollectionRepository : QueryRepository<Collections, CollectionFilters>, ICollectionRepository
    {
        public CollectionRepository(appDatabaseContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Collections>> GetCollectionsByUserId(int userId)
        {

            return await _context.Collections.Where(x => x.OwnerId == userId).ToListAsync();
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
