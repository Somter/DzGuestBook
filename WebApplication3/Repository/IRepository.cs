namespace WebApplication3.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task SaveAsync();
    }
}
