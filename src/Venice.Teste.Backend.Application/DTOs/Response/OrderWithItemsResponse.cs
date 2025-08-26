using System.Collections.Generic;

namespace Venice.Teste.Backend.Application.DTOs.Response
{
    public class OrderWithItemsResponse
    {
        public OrderResponse Order { get; set; } = default!;
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}