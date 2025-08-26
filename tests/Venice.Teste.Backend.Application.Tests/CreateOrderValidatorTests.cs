using FluentAssertions;
using Venice.Teste.Backend.Application.DTOs.Request;
using Venice.Teste.Backend.Application.UseCases.Order.Create;
using Xunit;

namespace Venice.Teste.Backend.Application.Tests;

public class CreateOrderValidatorTests
{
    [Fact]
    public void Should_Fail_When_Quantidade_Is_Less_Or_Equal_Zero()
    {
        var validator = new CommandValidator();
        var cmd = new Command(Guid.NewGuid(), new OrderRequest
        {
            Itens =
            {
                new OrderItemRequest { ProdutoId = Guid.NewGuid(), Quantidade = 0, PrecoUnitario = 10m }
            }
        });

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Quantidade deve ser maior que 0"));
    }

    [Fact]
    public void Should_Fail_When_PrecoUnitario_Is_Less_Or_Equal_Zero()
    {
        var validator = new CommandValidator();
        var cmd = new Command(Guid.NewGuid(), new OrderRequest
        {
            Itens =
            {
                new OrderItemRequest { ProdutoId = Guid.NewGuid(), Quantidade = 1, PrecoUnitario = 0m }
            }
        });

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("PrecoUnitario deve ser maior que 0"));
    }

    [Fact]
    public void Should_Fail_When_No_Items()
    {
        var validator = new CommandValidator();
        var cmd = new Command(Guid.NewGuid(), new OrderRequest());

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Pedido deve conter ao menos 1 item"));
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var validator = new CommandValidator();
        var cmd = new Command(Guid.NewGuid(), new OrderRequest
        {
            Itens =
            {
                new OrderItemRequest { ProdutoId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 15m }
            }
        });

        var result = validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }
}