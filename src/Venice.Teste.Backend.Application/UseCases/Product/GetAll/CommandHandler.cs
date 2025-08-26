using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Venice.Teste.Backend.Application.DTOs.Response;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Infrastructure.Repositories;

namespace Venice.Teste.Backend.Application.UseCases.Product.GetAll
{
    public class CommandHandler : IRequestHandler<Command, List<ProductResponse>>
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

        public async Task<List<ProductResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(Handle)}");
            var listCustomer = await _repository.Query()
                .Skip((request.PageOptions.Page - 1) * request.PageOptions.PageSize)
                .Take(request.PageOptions.PageSize)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();

            var result = listCustomer.Select(a => new ProductResponse
            {
                DataValidade = a.DataValidade,
                Valor = a.Valor,
                Quantidade = a.Quantidade,
                Nome = a.Nome,
                Id = a.Id,
            }).ToList();

            return result;
        }
    }
}