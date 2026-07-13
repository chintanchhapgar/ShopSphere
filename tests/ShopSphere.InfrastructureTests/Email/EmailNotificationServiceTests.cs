using FluentAssertions;
using Moq;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Infrastructure.Email.Helpers;
using ShopSphere.Infrastructure.Email.Services;

namespace ShopSphere.InfrastructureTests.Email;

public sealed class EmailNotificationServiceTests
{
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IEmailTemplateRenderer> _renderer = new();

    private readonly EmailNotificationService _service;

    public EmailNotificationServiceTests()
    {
        _service = new EmailNotificationService(
            _emailService.Object,
            _renderer.Object);
    }

    [Fact]
    public async Task SendOrderPlacedAsync_Should_Render_Template_And_Send_Email()
    {
        // Arrange
        var model = new OrderPlacedEmailModel(
            "John",
            "john@test.com",
            "ORD-0001",
            1500m);

        _renderer
            .Setup(x => x.RenderAsync(
                "OrderPlaced",
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>Order</html>");

        // Act
        await _service.SendOrderPlacedAsync(model);

        // Assert
        _renderer.Verify(x =>
            x.RenderAsync(
                "OrderPlaced",
                It.IsAny<Dictionary<string, string>>()),
            Times.Once);

        _emailService.Verify(x =>
            x.SendAsync(
                "john@test.com",
                "Your ShopSphere Order Confirmation",
                "<html>Order</html>",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendPaymentSucceededAsync_Should_Render_Template_And_Send_Email()
    {
        // Arrange
        var model = new PaymentSucceededEmailModel(
            "John",
            "john@test.com",
            "ORD-0001",
            1500m,
            "UPI",
            "TXN001");

        _renderer
            .Setup(x => x.RenderAsync(
                "PaymentSucceeded",
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>Payment</html>");

        // Act
        await _service.SendPaymentSucceededAsync(model);

        // Assert
        _emailService.Verify(x =>
            x.SendAsync(
                "john@test.com",
                "Payment Successful",
                "<html>Payment</html>",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendShipmentCreatedAsync_Should_Render_Template_And_Send_Email()
    {
        // Arrange
        var model = new ShipmentCreatedEmailModel(
            "John",
            "john@test.com",
            "ORD-0001",
            "TRK001",
            "BlueDart",
            DateTime.UtcNow.AddDays(2));

        _renderer
            .Setup(x => x.RenderAsync(
                "ShipmentCreated",
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>Shipment</html>");

        // Act
        await _service.SendShipmentCreatedAsync(model);

        // Assert
        _emailService.Verify(x =>
            x.SendAsync(
                "john@test.com",
                "Your Order Has Shipped",
                "<html>Shipment</html>",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendShipmentDeliveredAsync_Should_Render_Template_And_Send_Email()
    {
        // Arrange
        var model = new ShipmentDeliveredEmailModel(
            "John",
            "john@test.com",
            "ORD-0001",
            "TRK001");

        _renderer
            .Setup(x => x.RenderAsync(
                "ShipmentDelivered",
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>Delivered</html>");

        // Act
        await _service.SendShipmentDeliveredAsync(model);

        // Assert
        _emailService.Verify(x =>
            x.SendAsync(
                "john@test.com",
                "Order Delivered",
                "<html>Delivered</html>",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_Should_Render_Template_And_Send_Email()
    {
        // Arrange
        var model = new WelcomeEmailModel(
            "John Doe",
            "john@test.com");

        _renderer
            .Setup(x => x.RenderAsync(
                "Welcome",
                It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("<html>Welcome</html>");

        // Act
        await _service.SendWelcomeEmailAsync(model);

        // Assert
        _emailService.Verify(x =>
            x.SendAsync(
                "john@test.com",
                "Welcome to ShopSphere!",
                "<html>Welcome</html>",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SendOrderPlacedAsync_Should_Pass_Correct_Template_Data()
    {
        // Arrange
        Dictionary<string, string>? captured = null;

        _renderer
            .Setup(x => x.RenderAsync(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>()))
            .Callback<string, Dictionary<string, string>>((_, d) => captured = d)
            .ReturnsAsync("html");

        var model = new OrderPlacedEmailModel(
            "John",
            "john@test.com",
            "ORD-123",
            999.99m);

        // Act
        await _service.SendOrderPlacedAsync(model);

        // Assert
        captured.Should().NotBeNull();
        captured!["CustomerName"].Should().Be("John");
        captured["OrderNumber"].Should().Be("ORD-123");
    }
}