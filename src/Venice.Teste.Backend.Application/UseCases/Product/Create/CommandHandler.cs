using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Product.Create
{
    public class CommandHandler : IRequestHandler<Command, Guid>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CommandHandler(ILogger<CommandHandler> logger,
            IProductRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(Handle)}");
            var produto = _mapper.Map<Domain.Entities.Product>(request.ProductRequest);
            await _repository.AddAsync(produto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return produto.Id;
        }
    }
}