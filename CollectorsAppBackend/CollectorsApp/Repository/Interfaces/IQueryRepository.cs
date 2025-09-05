namespace CollectorsApp.Repository.Interfaces
{
    public interface IQueryInterface<T, TDTO> where T : class where TDTO : class
    {
        Task<IEnumerable<T>> QueryEntity(TDTO entity);
    }
}
