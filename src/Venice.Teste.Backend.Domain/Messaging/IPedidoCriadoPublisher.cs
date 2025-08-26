using Venice.Teste.Backend.Domain.Events;

namespace Venice.Teste.Backend.Domain.Messaging
{
    public interface IPedidoCriadoPublisher
    {
        Task PublishAsync(PedidoCriadoEvent evt, CancellationToken cancellationToken = default);
    }
}