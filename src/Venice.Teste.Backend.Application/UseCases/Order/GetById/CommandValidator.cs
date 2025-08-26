using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetById
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.id).NotEmpty();
        }
    }
}