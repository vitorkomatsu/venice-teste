using MediatR;
using Venice.Teste.Backend.Application.DTOs.Request;
using Venice.Teste.Backend.Application.DTOs.Response;

namespace Venice.Teste.Backend.Application.UseCases.Product.GetAll
{
    public record Command(Guid customerId, PageOptions PageOptions)
    : IRequest<List<ProductResponse>>;
}
