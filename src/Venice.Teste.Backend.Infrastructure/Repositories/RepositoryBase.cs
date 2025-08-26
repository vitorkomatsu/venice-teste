using Microsoft.EntityFrameworkCore;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.DbContexts;

namespace Venice.Teste.Backend.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _dbContext;

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual IQueryable<T> Query() => _dbContext.Set<T>().AsNoTracking().AsQueryable();

        public virtual IQueryable<T> TrackedQuery() => _dbContext.Set<T>().AsQueryable();

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }

        public virtual ValueTask<T> GetByIdAsync(Guid id) => _dbContext.Set<T>().FindAsync(id);

        public virtual Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
