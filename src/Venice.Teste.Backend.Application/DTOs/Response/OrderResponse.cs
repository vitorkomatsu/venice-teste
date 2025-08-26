using Venice.Teste.Backend.Domain.Enums;

namespace Venice.Teste.Backend.Application.DTOs.Response
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public DateTimeOffset Data { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
    }
}