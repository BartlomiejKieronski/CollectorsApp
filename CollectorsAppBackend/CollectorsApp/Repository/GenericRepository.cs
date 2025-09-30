using CollectorsApp.Data;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides a generic repository implementation for managing entities of type <typeparamref name="T"/>  in a
    /// database context. This class supports common data access operations such as retrieving, adding,  updating, and
    /// deleting entities.
    /// </summary>
    /// <remarks>This repository is designed to work with Entity Framework Core and assumes that the provided 
    /// <see cref="appDatabaseContext"/> is properly configured. It uses a <see cref="DbSet{TEntity}"/>  to perform
    /// operations on the specified entity type.</remarks>
    /// <typeparam name="T">The type of the entity managed by the repository. Must be a reference type.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly appDatabaseContext _context;
        protected readonly DbSet<T> _dbSet;
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class with the specified database
        /// context.
        /// </summary>
        /// <remarks>This constructor sets up the repository to operate on the specified entity type
        /// <typeparamref name="T"/>  using the provided <see cref="DbContext"/>. The repository will use the context to
        /// perform data operations.</remarks>
        /// <param name="context">The database context used to access the underlying data store. Cannot be <see langword="null"/>.</param>
        public GenericRepository(appDatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Deletes an entity with the specified identifier from the database.
        /// </summary>
        /// <remarks>If no entity with the specified identifier is found, the method returns <see
        /// langword="false"/>  and no changes are made to the database.</remarks>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Asynchronously retrieves all entities of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <remarks>This method queries the underlying database and returns all records as a collection
        /// of <typeparamref name="T"/>. The operation is performed asynchronously to avoid blocking the calling
        /// thread.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of all entities of type <typeparamref name="T"/> in the database. If no entities are found, the result is an
        /// empty collection.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <remarks>This method queries the underlying data source for an entity with the specified
        /// identifier.  If no entity with the given identifier exists, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
        /// name="T"/>  if found; otherwise, <see langword="null"/>.</returns>
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Asynchronously adds the specified entity to the database and saves the changes.
        /// </summary>
        /// <remarks>This method adds the entity to the underlying database context and commits the
        /// changes. Ensure that the entity is properly initialized and valid before calling this method.</remarks>
        /// <param name="entity">The entity to add to the database. Cannot be <see langword="null"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task PostAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the specified entity in the database asynchronously.
        /// </summary>
        /// <remarks>The entity's state is set to <see cref="EntityState.Modified"/> before saving changes
        /// to the database. Ensure that the provided entity is properly tracked by the context or has the same key as
        /// the existing entity in the database.</remarks>
        /// <param name="entity">The entity to be updated. The entity must already exist in the database.</param>
        /// <param name="Id">The identifier of the entity to be updated. This parameter is not used directly in the method but may be
        /// required for external validation or consistency checks.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task UpdateAsync(T entity, int Id)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Determines whether an entity with the specified identifier exists in the database.
        /// </summary>
        /// <remarks>This method performs an asynchronous lookup in the database to determine if an entity
        /// with the given identifier is present.</remarks>
        /// <param name="id">The unique identifier of the entity to check for existence.</param>
        /// <returns><see langword="true"/> if an entity with the specified identifier exists; otherwise, <see
        /// langword="false"/>.</returns>
        public virtual async Task<bool> Exists(int id)
        {
            var instance = await _dbSet.FindAsync(id);
            return instance != null;
        }
    }
}
