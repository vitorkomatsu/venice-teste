using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Infrastructure.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IMongoCollection<OrderItem> _collection;

        public OrderItemRepository(IMongoCollection<OrderItem> collection)
        {
            _collection = collection;
        }

        public async Task AddManyAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken)
        {
            await _collection.InsertManyAsync(items, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var filter = Builders<OrderItem>.Filter.Eq(i => i.OrderId, orderId);
            var results = await _collection.Find(filter).ToListAsync(cancellationToken);
            return results;
        }
    }
}