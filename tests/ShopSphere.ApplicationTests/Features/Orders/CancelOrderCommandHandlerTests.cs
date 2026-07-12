using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Orders.CancelOrder;
using ShopSphere.Application.Features.Orders.CreateOrder;
using ShopSphere.Application.Interfaces;
using ShopSphere.ApplicationTests.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.ApplicationTests.Features.Orders
{
    public class CancelOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepository = new();
        private readonly Mock<IInventoryRepository> _inventoryRepository = new();
        private readonly Mock<ICurrentUserService> _currentUserService = new();
        private readonly Mock<IAuditService> _auditService = new();

        private readonly CancelOrderCommandHandler _handler;

        public CancelOrderCommandHandlerTests()
        {
            _handler = new CancelOrderCommandHandler(
                _orderRepository.Object,
                _inventoryRepository.Object,
                _currentUserService.Object,
                _auditService.Object);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_User_Is_Unauthorized()
        {
            // Arrange
            _currentUserService
                .Setup(x => x.UserId)
                .Returns("invalid-guid");

            var command = new CancelOrderCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(UserErrors.Unauthorized);

            _orderRepository.Verify(
                x => x.GetByIdWithItemsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
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
        public async Task Handle_Should_ReturnFailure_When_Order_Not_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            var command = new CancelOrderCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(OrderErrors.NotFound);

            _orderRepository.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
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
        public async Task Handle_Should_ReturnFailure_When_Order_Belongs_To_Another_User()
        {
            // Arrange
            var currentUser = Guid.NewGuid();
            var owner = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(currentUser.ToString());

            var order = TestData.CreateOrder(owner);

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(OrderErrors.NotFound);

            _orderRepository.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_Order_Cannot_Be_Cancelled()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            var order = TestData.CreateOrder(userId);

            order.Confirm();

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Error.Should().Be(OrderErrors.CannotCancel);

            _orderRepository.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
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
        public async Task Handle_Should_Cancel_Order_Successfully()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            var order = TestData.CreateOrder(userId);

            var inventory = new Inventory(
                order.Items.First().ProductId,
                5,
                2);

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _inventoryRepository
                .Setup(x => x.GetByProductIdsAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([inventory]);

            // Act
            var result = await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            order.Status.Should().Be(OrderStatus.Cancelled);

            _orderRepository.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _auditService.Verify(
                x => x.LogAsync(
                    AuditActions.CancelOrder,
                    AuditEntities.Order,
                    order.Id,
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Restore_Inventory_For_All_Order_Items()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            var order = TestData.CreateOrder(userId);

            var item = order.Items.First();

            var inventory = new Inventory(
                item.ProductId,
                10,
                2);

            var originalQuantity = inventory.QuantityOnHand;

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _inventoryRepository
                .Setup(x => x.GetByProductIdsAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([inventory]);

            // Act
            await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            // Assert
            inventory.QuantityOnHand.Should()
                .Be(originalQuantity + item.Quantity);
        }

        [Fact]
        public async Task Handle_Should_SaveChanges_Once()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            var order = TestData.CreateOrder(userId);

            var inventory = new Inventory(
                order.Items.First().ProductId,
                10,
                2);

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _inventoryRepository
                .Setup(x => x.GetByProductIdsAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([inventory]);

            await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            _orderRepository.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Create_Audit_Log()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId.ToString());

            var order = TestData.CreateOrder(userId);

            var inventory = new Inventory(
                order.Items.First().ProductId,
                10,
                2);

            _orderRepository
                .Setup(x => x.GetByIdWithItemsAsync(
                    order.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _inventoryRepository
                .Setup(x => x.GetByProductIdsAsync(
                    It.IsAny<List<Guid>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([inventory]);

            await _handler.Handle(
                new CancelOrderCommand(order.Id),
                CancellationToken.None);

            _auditService.Verify(
                x => x.LogAsync(
                    AuditActions.CancelOrder,
                    AuditEntities.Order,
                    order.Id,
                    It.Is<string>(s => s.Contains(order.OrderNumber)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
