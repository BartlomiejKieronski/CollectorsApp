namespace CollectorsApp.Repository.Interfaces
{
    public interface ICRUD<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task UpdateAsync(T entity, int Id);
        Task PostAsync(T entity);
        Task<bool> Exists(int id);
    }
}