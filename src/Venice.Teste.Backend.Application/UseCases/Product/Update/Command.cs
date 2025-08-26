using MediatR;
using Venice.Teste.Backend.Application.DTOs.Request;

namespace Venice.Teste.Backend.Application.UseCases.Product.Update
{
    public record Command(Guid customerId, Guid Id, ProductRequest ProductRequest)
    : IRequest<Guid>;
}
