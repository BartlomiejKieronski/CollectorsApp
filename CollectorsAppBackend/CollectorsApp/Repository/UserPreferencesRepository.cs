using CollectorsApp.Data;
using CollectorsApp.Filters;
using CollectorsApp.Models;
using CollectorsApp.Repository.Interfaces;

namespace CollectorsApp.Repository
{
    /// <summary>
    /// Provides data access functionality for managing user preferences in the application.
    /// </summary>
    /// <remarks>This repository is responsible for querying and filtering user preferences based on the
    /// specified criteria. It extends the <see cref="QueryRepository{TEntity, TFilter}"/> class to provide generic
    /// query capabilities and implements the <see cref="IUserPreferencesRepository"/> interface to define
    /// application-specific behavior.</remarks>
    public class UserPreferencesRepository : QueryRepository<UserPreferences, UserPreferencesFilter>, IUserPreferencesRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPreferencesRepository"/> class with the specified database
        /// context.
        /// </summary>
        /// <remarks>The <paramref name="context"/> parameter must not be <see langword="null"/>. This
        /// repository provides methods for interacting with user preferences stored in the database.</remarks>
        /// <param name="context">The database context used to access and manage user preferences.</param>
        public UserPreferencesRepository(appDatabaseContext context) : base(context)
        {

        }
    }
}
