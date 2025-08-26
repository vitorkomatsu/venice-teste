using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.DbContexts;

namespace Venice.Teste.Backend.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
	{
        private readonly ApplicationDbContext _dbContext;
        private bool _isDisposed;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					_dbContext?.Dispose();
				}

				_isDisposed = true;
			}
		}
	}
}