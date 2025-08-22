using CollectorsApp.Data;
using CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces;

namespace CollectorsApp.Repository.AnalyticsRepositories
{
    public abstract class QuerryRepository<T, TDTO> : CRUDImplementation<T>, IQueryInterface<T, TDTO> where T : class where TDTO : class
    {

        public QuerryRepository(appDatabaseContext context) : base(context) { }
        public abstract Task<IEnumerable<T>> QueryEntity(TDTO entity);
        
    }
}