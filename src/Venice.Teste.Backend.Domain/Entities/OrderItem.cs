namespace Venice.Teste.Backend.Domain.Entities
{
    public class OrderItem
    {
        public Guid OrderId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}