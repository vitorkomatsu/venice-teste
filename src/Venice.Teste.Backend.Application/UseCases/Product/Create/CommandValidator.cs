using FluentValidation;

namespace Venice.Teste.Backend.Application.UseCases.Product.Create
{
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.ProductRequest)
                .NotEmpty()
                .NotNull();
        }
    }

    //public class CustomerRequestValidator : AbstractValidator<CustomerRequest>
    //{
    //    public CustomerRequestValidator()
    //    {
    //        RuleFor(x => x.Cnpj)
    //             .NotEmpty()
    //             .NotNull()
    //             .MaximumLength(14)
    //            .MinimumLength(14);
    //    }
    //}
}
