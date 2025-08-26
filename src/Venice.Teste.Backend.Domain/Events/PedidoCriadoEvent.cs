namespace Venice.Teste.Backend.Domain.Events
{
    public class OrderItemEvent
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class PedidoCriadoEvent
    {
        public Guid OrderId { get; set; }
        public Guid ClienteId { get; set; }
        public DateTimeOffset Data { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemEvent> Itens { get; set; } = new();
    }
}