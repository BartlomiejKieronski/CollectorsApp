namespace CollectorsApp.Repository.Interfaces
{
    public interface IQueryRepository<T, TDTO> where T : class where TDTO : class 
    {
        Task<IEnumerable<T>> QueryEntity(TDTO entity);
    }
}
