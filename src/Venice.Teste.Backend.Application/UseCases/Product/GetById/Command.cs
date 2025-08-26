using MediatR;
using Venice.Teste.Backend.Application.DTOs.Response;

namespace Venice.Teste.Backend.Application.UseCases.Product.GetById
{
    public record Command(Guid customerId, Guid id)
    : IRequest<ProductResponse>;
}
