using FluentAssertions;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Enums;
using Xunit;

namespace Venice.Teste.Backend.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void SetTotal_ShouldUpdateTotal()
    {
        // Arrange
        var order = new Order
        {
            ClienteId = Guid.NewGuid(),
            Data = DateTimeOffset.UtcNow,
            Status = OrderStatus.Criado
        };

        // Act
        order.SetTotal(123.45m);

        // Assert
        order.Total.Should().Be(123.45m);
    }

    [Fact]
    public void NewOrder_ShouldHaveNonEmptyId()
    {
        // Arrange & Act
        var order = new Order();

        // Assert
        order.Id.Should().NotBe(Guid.Empty);
    }
}