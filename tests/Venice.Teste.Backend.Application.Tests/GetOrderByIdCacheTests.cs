using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using Venice.Teste.Backend.Application.DTOs.Response;
using Venice.Teste.Backend.Application.UseCases.Order.GetById;
using Venice.Teste.Backend.Domain.Entities;
using Venice.Teste.Backend.Domain.Repositories;
using Venice.Teste.Backend.Domain.Services;
using Xunit;

namespace Venice.Teste.Backend.Application.Tests;

public class GetOrderByIdCacheTests
{
    private static CommandHandler CreateHandler(
        Mock<IOrderRepository> orderRepoMock,
        Mock<IOrderItemRepository> itemRepoMock,
        Mock<ICacheService> cacheMock)
    {
        var logger = Mock.Of<ILogger<CommandHandler>>();
        return new CommandHandler(logger, orderRepoMock.Object, itemRepoMock.Object, cacheMock.Object);
    }

    [Fact]
    public async Task Handle_Should_LoadFromRepo_And_SetCache_When_CacheMiss()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order { ClienteId = Guid.NewGuid(), Total = 50m };
        order.GetType().GetProperty("Id")!.SetValue(order, orderId);

        var ordersQueryable = new List<Order> { order }
            .AsQueryable()
            .BuildMock();

        var items = new List<OrderItem>
        {
            new OrderItem { OrderId = orderId, ProdutoId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 25m, Subtotal = 50m }
        };

        var orderRepoMock = new Mock<IOrderRepository>();
        orderRepoMock.Setup(r => r.Query()).Returns(ordersQueryable.Object);

        var itemRepoMock = new Mock<IOrderItemRepository>();
        itemRepoMock.Setup(r => r.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var cacheMock = new Mock<ICacheService>(MockBehavior.Strict);
        cacheMock.Setup(c => c.GetAsync<OrderWithItemsResponse>($"order:{orderId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderWithItemsResponse?)null);

        TimeSpan? ttlCaptured = null;
        cacheMock.Setup(c => c.SetAsync($"order:{orderId}", It.IsAny<OrderWithItemsResponse>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Callback<string, OrderWithItemsResponse, TimeSpan, CancellationToken>((_, __, ttl, ___) => ttlCaptured = ttl)
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(orderRepoMock, itemRepoMock, cacheMock);

        // Act
        var result = await handler.Handle(new Command(orderId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Order.Id.Should().Be(orderId);
        result.Items.Should().HaveCount(1);
        ttlCaptured.Should().Be(TimeSpan.FromSeconds(120));

        orderRepoMock.Verify(r => r.Query(), Times.Once);
        itemRepoMock.Verify(r => r.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.Verify(c => c.SetAsync($"order:{orderId}", It.IsAny<OrderWithItemsResponse>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFromCache_When_CacheHit()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var cached = new OrderWithItemsResponse
        {
            Order = new OrderResponse { Id = orderId, ClienteId = Guid.NewGuid(), Data = DateTimeOffset.UtcNow, Total = 10m },
            Items = new List<OrderItemResponse>()
        };

        var orderRepoMock = new Mock<IOrderRepository>(MockBehavior.Strict);
        var itemRepoMock = new Mock<IOrderItemRepository>(MockBehavior.Strict);

        var cacheMock = new Mock<ICacheService>(MockBehavior.Strict);
        cacheMock.Setup(c => c.GetAsync<OrderWithItemsResponse>($"order:{orderId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cached);

        var handler = CreateHandler(orderRepoMock, itemRepoMock, cacheMock);

        // Act
        var result = await handler.Handle(new Command(orderId), CancellationToken.None);

        // Assert
        result.Should().BeSameAs(cached);
        cacheMock.Verify(c => c.GetAsync<OrderWithItemsResponse>($"order:{orderId}", It.IsAny<CancellationToken>()), Times.Once);
        cacheMock.VerifyNoOtherCalls();
        orderRepoMock.VerifyNoOtherCalls();
        itemRepoMock.VerifyNoOtherCalls();
    }
}