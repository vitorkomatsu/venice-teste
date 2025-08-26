using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Product.GetAll
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.PageOptions.Page)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.PageOptions.PageSize)
                .NotEmpty()
                .NotNull();
        }
    }
}
