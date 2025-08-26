using MediatR;
using Venice.Teste.Backend.Application.DTOs.Response;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetById
{
    public record Command(Guid id) : IRequest<OrderWithItemsResponse>;
}