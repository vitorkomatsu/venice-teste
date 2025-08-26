using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.DbContexts;

namespace Venice.Teste.Backend.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase<Domain.Entities.Product>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
		}
    }
}