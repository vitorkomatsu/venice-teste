using Venice.Teste.Backend.Domain.Enums;

namespace Venice.Teste.Backend.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid ClienteId { get; set; }
        public DateTimeOffset Data { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Criado;
        public decimal Total { get; set; }

        public void SetTotal(decimal total)
        {
            Total = total;
        }
    }
}