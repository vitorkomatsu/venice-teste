using MediatR;
using Venice.Teste.Backend.Application.DTOs.Request;

namespace Venice.Teste.Backend.Application.UseCases.Product.Create
{
    public record Command(Guid customerId, ProductRequest ProductRequest)
    : IRequest<Guid>;
}
