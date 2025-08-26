using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Product.Delete
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.id)
                .NotEmpty()
                .NotNull();
        }
    }
}
