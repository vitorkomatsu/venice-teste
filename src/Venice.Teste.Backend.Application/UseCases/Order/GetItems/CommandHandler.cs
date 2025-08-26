using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Application.DTOs.Response;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetItems
{
    public class CommandHandler : IRequestHandler<Command, List<OrderItemResponse>>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMapper _mapper;

        public CommandHandler(ILogger<CommandHandler> logger,
            IOrderItemRepository orderItemRepository,
            IMapper mapper)
        {
            _logger = logger;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderItemResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Handler}", nameof(Handle));
            var items = await _orderItemRepository.GetByOrderIdAsync(request.orderId, cancellationToken);
            var result = _mapper.Map<List<OrderItemResponse>>(items);
            return result;
        }
    }
}