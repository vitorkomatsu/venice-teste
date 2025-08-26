using Venice.Teste.Backend.Domain.Entities;

namespace Venice.Teste.Backend.Domain.Repositories
{
    public interface IOrderItemRepository
    {
        Task AddManyAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken);
        Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    }
}