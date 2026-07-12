using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Orders.GetOrderById;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;

public sealed class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderQueries> _orderQueries = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _handler = new GetOrderByIdQueryHandler(
            _orderQueries.Object,
            _currentUserService.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_User_Is_Unauthorized()
    {
        // Arrange
        _currentUserService
            .Setup(x => x.UserId)
            .Returns("invalid-guid");

        // Act
        var result = await _handler.Handle(
            new GetOrderByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(UserErrors.Unauthorized);

        _orderQueries.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Order_Not_Found()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        _orderQueries
            .Setup(x => x.GetByIdAsync(
                customerId,
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderDetailsDto?)null);

        // Act
        var result = await _handler.Handle(
            new GetOrderByIdQuery(orderId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(OrderErrors.NotFound);
    }
    [Fact]
    public async Task Handle_Should_Return_Order_When_Order_Exists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var dto = new OrderDetailsDto(
    orderId,
    "ORD-0001",
    DateTime.UtcNow,
    "Pending",
    100m,
    10m,
    5m,
    0m,
    115m,
    "John Doe",
    "9999999999",
    "Street 1",
    null,
    "Surat",
    "Gujarat",
    "395007",
    "India",
    new List<OrderItemDto>());

        _orderQueries
            .Setup(x => x.GetByIdAsync(
                customerId,
                orderId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        // Act
        var result = await _handler.Handle(
            new GetOrderByIdQuery(orderId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

        result.Value!.Id.Should().Be(orderId);

        result.Value.OrderNumber.Should().Be("ORD-0001");

        _orderQueries.Verify(
            x => x.GetByIdAsync(
                customerId,
                orderId,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}