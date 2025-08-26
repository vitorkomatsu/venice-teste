using Venice.Teste.Backend.Domain.Entities;

namespace Venice.Teste.Backend.Domain.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        ValueTask<T> GetByIdAsync(Guid id);
        IQueryable<T> Query();
        IQueryable<T> TrackedQuery();
        Task<T> AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);
    }
}