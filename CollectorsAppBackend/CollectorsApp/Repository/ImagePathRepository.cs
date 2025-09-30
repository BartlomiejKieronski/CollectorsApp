using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides data access functionality for managing image paths in the database.
    /// </summary>
    /// <remarks>This repository is responsible for querying and filtering image path data, including
    /// retrieving image paths associated with specific items. It extends the <see cref="QueryRepository{TEntity,
    /// TFilter}"/> base class to provide common query operations and implements the <see cref="IImagePathRepository"/>
    /// interface for additional image path-specific functionality.</remarks>
    public class ImagePathRepository : QueryRepository<ImagePath, ImagePathFilter>, IImagePathRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePathRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <remarks>This constructor initializes the repository by passing the provided database context
        /// to the base class. Ensure that the <paramref name="context"/> is properly configured before using this
        /// repository.</remarks>
        /// <param name="context">The <see cref="appDatabaseContext"/> instance used to interact with the database.  This parameter cannot be
        /// <see langword="null"/>.</param>
        public ImagePathRepository(appDatabaseContext context) :base(context) 
        {
            
        }

        /// <summary>
        /// Retrieves a collection of image paths associated with the specified item ID.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item whose image paths are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{T} of ImagePath
        /// objects associated with the specified item ID. If no image paths are found, an empty collection is returned.</returns>
        public async Task<IEnumerable<ImagePath>> GetImagePathsByItemId(int itemId)
        {
            return await _context.ImagePaths.Where(x=>x.ItemId == itemId).ToListAsync();
        }
    }
}
