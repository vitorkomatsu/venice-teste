using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Application.DTOs.Response;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Domain.Services;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetById
{
    public class CommandHandler : IRequestHandler<Command, OrderWithItemsResponse>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICacheService _cache;

        public CommandHandler(ILogger<CommandHandler> logger,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICacheService cache)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cache = cache;
        }

        public async Task<OrderWithItemsResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}", nameof(Handle));

            var cacheKey = $"order:{request.id}";
            var cached = await _cache.GetAsync<OrderWithItemsResponse>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return cached;
            }

            var order = await _orderRepository.Query().FirstOrDefaultAsync(o => o.Id == request.id, cancellationToken);
            if (order is null)
                throw new KeyNotFoundException("Pedido não encontrado");

            var items = await _orderItemRepository.GetByOrderIdAsync(request.id, cancellationToken);

            var response = new OrderWithItemsResponse
            {
                Order = new OrderResponse
                {
                    Id = order.Id,
                    ClienteId = order.ClienteId,
                    Data = order.Data,
                    Status = order.Status,
                    Total = order.Total
                },
                Items = items.Select(i => new OrderItemResponse
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromSeconds(120), cancellationToken);
            return response;
        }
    }
}