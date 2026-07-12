using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Orders.Admin.GetOrders;
using ShopSphere.Application.Queries;

public sealed class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderQueries> _orderQueries = new();

    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _handler = new GetOrdersQueryHandler(
            _orderQueries.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Orders()
    {
        // Arrange
        var orders = new List<AdminOrderListDto>
    {
        new(
            Guid.NewGuid(),
            "ORD-0001",
            Guid.NewGuid(),
            "Pending",
            1000m,
            2,
            DateTime.UtcNow),

        new(
            Guid.NewGuid(),
            "ORD-0002",
            Guid.NewGuid(),
            "Delivered",
            2500m,
            5,
            DateTime.UtcNow)
    };

        _orderQueries
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);

        // Act
        var result = await _handler.Handle(
            new GetOrdersQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().HaveCount(2);

        result.Value![0].OrderNumber.Should().Be("ORD-0001");

        result.Value[1].Status.Should().Be("Delivered");

        _orderQueries.Verify(
            x => x.GetAllAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Orders()
    {
        // Arrange
        _orderQueries
            .Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdminOrderListDto>());

        // Act
        var result = await _handler.Handle(
            new GetOrdersQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

        result.Value.Should().BeEmpty();
    }
}