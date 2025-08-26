using MediatR;
using Venice.Teste.Backend.Application.DTOs.Response;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetItems
{
    public record Command(Guid orderId) : IRequest<List<OrderItemResponse>>;
}