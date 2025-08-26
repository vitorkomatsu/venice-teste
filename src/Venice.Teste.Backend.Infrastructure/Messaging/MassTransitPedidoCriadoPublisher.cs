using MassTransit;
using Venice.Teste.Backend.Domain.Events;
using Venice.Teste.Backend.Domain.Messaging;

namespace Venice.Teste.Backend.Infrastructure.Messaging
{
    public class MassTransitPedidoCriadoPublisher : IPedidoCriadoPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitPedidoCriadoPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync(PedidoCriadoEvent evt, CancellationToken cancellationToken = default)
        {
            return _publishEndpoint.Publish(evt, cancellationToken);
        }
    }
}