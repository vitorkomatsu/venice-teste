using System.ComponentModel.DataAnnotations;

namespace Venice.Teste.Backend.Application.DTOs.Request
{
    public class OrderRequest
    {
        [Required]
        public List<OrderItemRequest> Itens { get; set; } = new();
    }
}