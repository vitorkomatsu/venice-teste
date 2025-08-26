using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Domain.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Product.Update
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
            var produto = await _repository.GetByIdAsync(request.Id);
            if (produto is null)
                throw new KeyNotFoundException("Product not found");

            produto = _mapper.Map(request.ProductRequest, produto);
            await _repository.UpdateAsync(produto);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return produto.Id;
        }
    }
}