using MediatR;
using Venice.Teste.Backend.Application.DTOs.Request;

namespace Venice.Teste.Backend.Application.UseCases.Order.Create
{
    public record Command(Guid customerId, OrderRequest Request) : IRequest<Guid>;
}