using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Venice.Teste.Backend.Application.DTOs.Request;
using Venice.Teste.Backend.Application.UseCases.Order.Create;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Messaging;
using Venice.Teste.Backend.Domain.Repositories;
using Xunit;

namespace Venice.Teste.Backend.Application.Tests;

public class CreateOrderHandlerTests
{
    [Fact]
    public async Task Handle_Should_Calculate_Total_And_Persist_Items_And_Publish_Event()
    {
        // Arrange
        var orderRepo = new Mock<IOrderRepository>();
        var itemRepo = new Mock<IOrderItemRepository>();
        var uow = new Mock<IUnitOfWork>();
        var publisher = new Mock<IPedidoCriadoPublisher>();
        var logger = Mock.Of<ILogger<CommandHandler>>();

        Order? updatedOrder = null;
        orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);
        orderRepo.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .Callback<Order>(o => updatedOrder = o)
            .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        IReadOnlyList<OrderItem>? persistedItems = null;
        itemRepo.Setup(r => r.AddManyAsync(It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<OrderItem>, CancellationToken>((items, _) => persistedItems = items.ToList())
            .Returns(Task.CompletedTask);

        publisher.Setup(p => p.PublishAsync(It.IsAny<Domain.Events.PedidoCriadoEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CommandHandler(logger, orderRepo.Object, itemRepo.Object, uow.Object, publisher.Object);

        var cmd = new Command(Guid.NewGuid(), new OrderRequest
        {
            Itens =
            {
                new OrderItemRequest { ProdutoId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 10m }, // 20
                new OrderItemRequest { ProdutoId = Guid.NewGuid(), Quantidade = 3, PrecoUnitario = 5m }   // 15
            }
        });

        // Act
        var orderId = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        orderId.Should().NotBe(Guid.Empty);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Total.Should().Be(35m);

        persistedItems.Should().NotBeNull();
        persistedItems!.Count.Should().Be(2);
        persistedItems.Sum(i => i.Subtotal).Should().Be(35m);

        orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        orderRepo.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
        itemRepo.Verify(r => r.AddManyAsync(It.IsAny<IEnumerable<OrderItem>>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        publisher.Verify(p => p.PublishAsync(It.IsAny<Domain.Events.PedidoCriadoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}