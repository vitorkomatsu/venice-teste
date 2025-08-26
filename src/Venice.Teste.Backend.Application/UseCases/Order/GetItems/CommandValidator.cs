using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Order.GetItems
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.orderId).NotEmpty();
        }
    }
}