using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.DbContexts;

namespace Venice.Teste.Backend.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}