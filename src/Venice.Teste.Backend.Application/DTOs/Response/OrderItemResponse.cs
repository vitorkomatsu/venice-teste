namespace Venice.Teste.Backend.Application.DTOs.Response
{
    public class OrderItemResponse
    {
        public Guid OrderId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}