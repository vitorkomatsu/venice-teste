using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Order.Create
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.customerId).NotEmpty();
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.Itens)
                .NotEmpty()
                .WithMessage("Pedido deve conter ao menos 1 item");

            RuleForEach(x => x.Request.Itens).ChildRules(items =>
            {
                items.RuleFor(i => i.Quantidade)
                    .GreaterThan(0)
                    .WithMessage("Quantidade deve ser maior que 0");
                items.RuleFor(i => i.PrecoUnitario)
                    .GreaterThan(0)
                    .WithMessage("PrecoUnitario deve ser maior que 0");
            });
        }
    }
}