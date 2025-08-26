using MediatR;

namespace Venice.Teste.Backend.Application.UseCases.Product.Delete
{
    public record Command(Guid customerId, Guid id)
    : IRequest<Guid>;
}
