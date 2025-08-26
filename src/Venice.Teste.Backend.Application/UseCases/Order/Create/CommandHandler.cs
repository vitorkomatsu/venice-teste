using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Enums;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Domain.Messaging;
using Venice.Teste.Backend.Domain.Events;

namespace Venice.Teste.Backend.Application.UseCases.Order.Create
{
    public class CommandHandler : IRequestHandler<Command, Guid>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPedidoCriadoPublisher _publisher;

        public CommandHandler(ILogger<CommandHandler> logger,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IUnitOfWork unitOfWork,
            IPedidoCriadoPublisher publisher)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}", nameof(Handle));

            // 1) Persist Order in SQL
            var order = new Domain.Entities.Order
            {
                ClienteId = request.customerId,
                Data = DateTimeOffset.UtcNow,
                Status = OrderStatus.Criado
            };

            decimal total = 0m;
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var items = new List<OrderItem>();
            foreach (var i in request.Request.Itens)
            {
                var subtotal = i.PrecoUnitario * i.Quantidade;
                total += subtotal;
                items.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = subtotal
                });
            }

            order.SetTotal(total);
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 2) Persist items to Mongo
            if (items.Count > 0)
            {
                await _orderItemRepository.AddManyAsync(items, cancellationToken);
            }

            // 3) Publish event
            var evt = new PedidoCriadoEvent
            {
                OrderId = order.Id,
                ClienteId = order.ClienteId,
                Data = order.Data,
                Total = order.Total,
                Itens = items.Select(x => new OrderItemEvent
                {
                    ProdutoId = x.ProdutoId,
                    Quantidade = x.Quantidade,
                    PrecoUnitario = x.PrecoUnitario,
                    Subtotal = x.Subtotal
                }).ToList()
            };
            await _publisher.PublishAsync(evt, cancellationToken);

            return order.Id;
        }
    }
}