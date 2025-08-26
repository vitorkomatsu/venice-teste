using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Product.Delete
{
    public class CommandHandler : IRequestHandler<Command, Guid>
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IProductRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public CommandHandler(ILogger<CommandHandler> logger,
            IProductRepository repository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(Handle)}.{request.id}");
            var product = await _repository.GetByIdAsync(request.id);
            if (product is null)
                throw new KeyNotFoundException("Product not found");

            await _repository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return product.Id;
        }
    }
}