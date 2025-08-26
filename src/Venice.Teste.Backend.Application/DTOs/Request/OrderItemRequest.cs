namespace Venice.Teste.Backend.Application.DTOs.Request
{
    public class OrderItemRequest
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}