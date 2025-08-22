namespace CollectorsApp.Repository.AnalyticsRepositories.AnalyticsRepositoryInterfaces
{
    public interface IQueryInterface<T, TDTO> where T : class where TDTO : class
    {
        abstract Task<IEnumerable<T>> QueryEntity(TDTO entity);
    }
}
