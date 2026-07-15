using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Orders.CreateOrder;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.ApplicationTests.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;
using Xunit;

namespace ShopSphere.ApplicationTests.Features.Orders;

public sealed class CreateOrderCommandHandlerTests
{
    private readonly Mock<ICartRepository> _cartRepository = new();

    private readonly Mock<IInventoryRepository> _inventoryRepository = new();

    private readonly Mock<IOrderRepository> _orderRepository = new();

    private readonly Mock<IAddressRepository> _addressRepository = new();

    private readonly Mock<ICurrentUserService> _currentUserService = new();

    private readonly Mock<IUserService> _userService = new();

    private readonly Mock<INotificationService> _notificationService = new();

    private readonly Mock<IBackgroundJobService> _backgroundJobs = new();

    private readonly Mock<IAuditService> _auditService = new();

    private readonly Mock<ILogger<CreateOrderCommandHandler>> _logger = new();

    private readonly CreateOrderCommandHandler _handler;
    private readonly Mock<IPushNotificationService> _pushNotificationService = new();
    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler(
            _cartRepository.Object,
            _inventoryRepository.Object,
            _orderRepository.Object,
            _addressRepository.Object,
            _currentUserService.Object,
            _userService.Object,
            _notificationService.Object,
            _backgroundJobs.Object,
            _auditService.Object,
            _logger.Object,
            _pushNotificationService.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserId_Is_Invalid()
    {
        _currentUserService
            .Setup(x => x.UserId)
            .Returns("abc");

        var command = new CreateOrderCommand(Guid.NewGuid());

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Cart_Not_Found()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShopSphere.Domain.Entities.Cart?)null);

        var command = new CreateOrderCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CartErrors.Empty);

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Cart_Is_Empty()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var cart = new Cart(customerId);

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var command = new CreateOrderCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CartErrors.Empty);

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);

        _auditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Inventory_Not_Found()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var product = new Product(
            "iPhone 16",
            "Apple Phone",
            "IPH-001",
            1000,
            800,
            categoryId,
            brandId,
            "iphone-16",
            null,
            null);

        var cart = TestData.CreateCart(customerId, product);

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Inventory>());

        var command = new CreateOrderCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InventoryErrors.NotFound);

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);

        _auditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Inventory_Is_Insufficient()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            categoryId,
            brandId);

        var cart = TestData.CreateCart(
            customerId,
            product,
            quantity: 2);

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Inventory has LESS stock than cart requires
        var inventory = new Inventory(
            product.Id,
            quantityOnHand: 1,
            lowStockThreshold: 5);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        var address = CreateAddress(customerId);

        _addressRepository
            .Setup(x => x.GetByIdAsync(
                address.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        var command = new CreateOrderCommand(address.Id);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(InventoryErrors.InsufficientStock);

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);

        _auditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Address_Not_Found()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var product = new Product(
            "iPhone 16",
            "Apple Phone",
            "IPH-001",
            1000,
            800,
            categoryId,
            brandId,
            "iphone-16",
            null,
            null);

        var cart = TestData.CreateCart(customerId, product);

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        var inventory = new Inventory(
    product.Id,
    20,
    5);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Address?)null);

        var command = new CreateOrderCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(AddressErrors.NotFound);

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);

        _auditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Address CreateAddress(Guid customerId)
    {
        return new Address(
            customerId,
            "John Doe",
            "9876543210",
            "123 Main Street",
            null,
            "Surat",
            "Gujarat",
            "395001",
            "India");
    }

    public async Task Handle_Should_Create_Order_Successfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _currentUserService
            .Setup(x => x.UserId)
            .Returns(customerId.ToString());

        var categoryId = Guid.NewGuid();
        var brandId = Guid.NewGuid();

        var product = new Product(
            "iPhone 16",
            "Apple Phone",
            "IPH001",
            1000,
            800,
            categoryId,
            brandId,
            "iphone-16",
            null,
            null);

        var cart = TestData.CreateCart(customerId, product);

        var inventory = new Inventory(
            product.Id,
            20,
            5);

        var address = CreateAddress(customerId);

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(
                customerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(
                address.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        _orderRepository
            .Setup(x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _orderRepository
            .Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new CreateOrderCommand(address.Id);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

        _orderRepository.Verify(
            x => x.AddAsync(
                It.IsAny<Order>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _orderRepository.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        _auditService.Verify(
            x => x.LogAsync(
                AuditActions.CreateOrder,
                AuditEntities.Order,
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Apply_Coupon_When_Cart_Has_Coupon()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            Guid.NewGuid(),
            Guid.NewGuid());

        var cart = TestData.CreateCart(customerId, product);        

        var coupon = new Coupon(
            "SAVE10",
            "Save 10",
            null,
            DiscountType.FixedAmount,
            10,
            0,
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(10),
            100);

        cart.ApplyCoupon(coupon);

        var inventory = new Inventory(product.Id, 10, 2);

        var address = CreateAddress(customerId);

        _currentUserService.Setup(x => x.UserId).Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        // Act
        await _handler.Handle(
            new CreateOrderCommand(address.Id),
            CancellationToken.None);

        // Assert
        cart.Coupon.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_Decrease_Inventory_For_All_Items()
    {
        var customerId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            Guid.NewGuid(),
            Guid.NewGuid());

        var cart = TestData.CreateCart(customerId, product);

        var inventory = new Inventory(product.Id, 10, 2);

        

        var address = CreateAddress(customerId);

        _currentUserService.Setup(x => x.UserId).Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        await _handler.Handle(
            new CreateOrderCommand(address.Id),
            CancellationToken.None);

        inventory.QuantityOnHand.Should().Be(8);
    }

    [Fact]
    public async Task Handle_Should_Clear_Cart_After_Order()
    {
        var customerId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            Guid.NewGuid(),
            Guid.NewGuid());

        var cart = TestData.CreateCart(customerId, product);

        var inventory = new Inventory(product.Id, 5, 1);

        var address = CreateAddress(customerId);

        _currentUserService.Setup(x => x.UserId).Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        await _handler.Handle(
            new CreateOrderCommand(address.Id),
            CancellationToken.None);

        _cartRepository.Verify(
            x => x.RemoveItems(cart),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Create_Audit_Log()
    {
        var customerId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            Guid.NewGuid(),
            Guid.NewGuid());

        var cart = TestData.CreateCart(customerId, product);

        var inventory = new Inventory(product.Id, 10, 1);

        var address = CreateAddress(customerId);

        _currentUserService.Setup(x => x.UserId).Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        await _handler.Handle(
            new CreateOrderCommand(address.Id),
            CancellationToken.None);

        _auditService.Verify(
            x => x.LogAsync(
                AuditActions.CreateOrder,
                AuditEntities.Order,
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Queue_OrderConfirmation_Email()
    {
        var customerId = Guid.NewGuid();

        var product = TestData.CreateProduct(
            Guid.NewGuid(),
            Guid.NewGuid());

        var cart = TestData.CreateCart(customerId, product);

        var inventory = new Inventory(product.Id, 10, 1);

        var address = CreateAddress(customerId);

        _currentUserService.Setup(x => x.UserId).Returns(customerId.ToString());

        _cartRepository
            .Setup(x => x.GetByCustomerWithItemsAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        _inventoryRepository
            .Setup(x => x.GetByProductIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([inventory]);

        _addressRepository
            .Setup(x => x.GetByIdAsync(address.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(address);

        await _handler.Handle(
            new CreateOrderCommand(address.Id),
            CancellationToken.None);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }
}