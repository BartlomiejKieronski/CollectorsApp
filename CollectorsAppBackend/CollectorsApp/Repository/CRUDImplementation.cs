using CollectorsApp.Data;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    public class CRUDImplementation<T> : ICRUD<T> where T : class
    {
        protected readonly appDatabaseContext _context;
        protected readonly DbSet<T> _dbSet;
        public CRUDImplementation(appDatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var instance = await _dbSet.FindAsync(id);
            if (instance != null)
            {
                _dbSet.Remove(instance);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task PostAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity, int Id)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task<bool> Exists(int id)
        {
            var instance = await _dbSet.FindAsync(id);
            return instance != null;
        }
    }
}
