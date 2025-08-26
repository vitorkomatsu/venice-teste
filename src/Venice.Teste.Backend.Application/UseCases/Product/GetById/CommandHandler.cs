using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Application.DTOs.Response;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Product.GetById
{
    public class CommandHandler : IRequestHandler<Command, ProductResponse>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        public CommandHandler(ILogger<CommandHandler> logger,
            IProductRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(Handle)}");
            var produto = await _repository.GetByIdAsync(request.id);
            if (produto is null)
                throw new KeyNotFoundException("Produto not found");

            var result = _mapper.Map<ProductResponse> (produto);
            return result;
        }
    }
}