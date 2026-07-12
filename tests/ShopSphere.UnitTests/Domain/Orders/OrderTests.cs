using FluentAssertions;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;

namespace ShopSphere.UnitTests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void Create_Should_SetPendingStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var order = Order.Create(
            userId,
            "ORD-1001",
            "John Doe",
            "9999999999",
            "Address Line 1",
            null,
            "Surat",
            "Gujarat",
            "395001",
            "India");

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.UserId.Should().Be(userId);
        order.OrderNumber.Should().Be("ORD-1001");
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void Cancel_Should_ChangeStatus_ToCancelled()
    {
        // Arrange
        var order = Order.Create(
            Guid.NewGuid(),
            "ORD-1001",
            "John Doe",
            "9999999999",
            "Address Line 1",
            null,
            "Surat",
            "Gujarat",
            "395001",
            "India");

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Confirm_Should_ChangeStatus_ToConfirmed()
    {
        // Arrange
        var order = CreateOrder();

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_Should_Throw_When_Order_Is_NotPending()
    {
        // Arrange
        var order = CreateOrder();

        order.Confirm();

        // Act
        var action = () => order.Confirm();

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Only pending orders can be confirmed.");
    }

    [Fact]
    public void StartProcessing_Should_ChangeStatus_ToProcessing()
    {
        // Arrange
        var order = CreateOrder();

        order.Confirm();

        // Act
        order.StartProcessing();

        // Assert
        order.Status.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public void MarkShipped_Should_ChangeStatus_ToShipped()
    {
        // Arrange
        var order = CreateOrder();

        order.Confirm();
        order.StartProcessing();

        // Act
        order.MarkShipped();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void MarkDelivered_Should_ChangeStatus_ToDelivered()
    {
        // Arrange
        var order = CreateOrder();

        order.Confirm();
        order.StartProcessing();
        order.MarkShipped();

        // Act
        order.MarkDelivered();

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Complete_Should_ChangeStatus_ToCompleted()
    {
        // Arrange
        var order = CreateOrder();

        order.Confirm();
        order.StartProcessing();
        order.MarkShipped();
        order.MarkDelivered();

        // Act
        order.Complete();

        // Assert
        order.Status.Should().Be(OrderStatus.Completed);
    }
    private static Order CreateOrder()
    {
        return Order.Create(
            Guid.NewGuid(),
            "ORD-1001",
            "John Doe",
            "9999999999",
            "Address Line 1",
            null,
            "Surat",
            "Gujarat",
            "395001",
            "India");
    }

    [Fact]
    public void AddItem_Should_RecalculateSubtotal()
    {
        // Arrange
        var order = CreateOrder();

        var item = OrderItem.Create(
             Guid.NewGuid(),
             "iPhone 16",
             "IPH16",
             null,
             50000m,
             2);

        // Act
        order.AddItem(item);

        // Assert
        order.SubTotal.Should().Be(100000);
        order.TotalAmount.Should().Be(100000);
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public void AddMultipleItems_Should_CalculateSubtotal()
    {
        // Arrange
        var order = CreateOrder();

        order.AddItem(OrderItem.Create(
            Guid.NewGuid(),
            "Product 1",
            "P001",
            null,
            100m,
            2));

        order.AddItem(OrderItem.Create(
            Guid.NewGuid(),
            "Product 2",
            "P002",
            null,
            50m,
            3));

        // Assert
        order.SubTotal.Should().Be(350);
        order.TotalAmount.Should().Be(350);
    }

    [Fact]
    public void SetCharges_Should_AddTax()
    {
        // Arrange
        var order = CreateOrder();

        order.AddItem(OrderItem.Create(
             Guid.NewGuid(),
             "Product",
             "P001",
             null,
             1000m,
             1));

        // Act
        order.SetCharges(
            taxAmount: 180,
            shippingAmount: 0);

        // Assert
        order.TotalAmount.Should().Be(1180);
    }
    [Fact]
    public void SetCharges_Should_AddShipping()
    {
        var order = CreateOrder();

        order.AddItem(OrderItem.Create(
            Guid.NewGuid(),
            "Product",
            "P001",
            null,
            1000m,
            1));

        order.SetCharges(
            taxAmount: 0,
            shippingAmount: 100);

        order.TotalAmount.Should().Be(1100);
    }

    [Fact]
    public void SetCharges_Should_ApplyDiscount()
    {
        var order = CreateOrder();

        order.AddItem(OrderItem.Create(
            Guid.NewGuid(),
            "Product",
            "P001",
            null,
            1000m,
            1));

        order.SetCharges(
            taxAmount: 100,
            shippingAmount: 50,
            discountAmount: 200);

        order.TotalAmount.Should().Be(950);
    }

    [Fact]
    public void ApplyCoupon_Should_SetCouponDetails()
    {
        var order = CreateOrder();

        var couponId = Guid.NewGuid();

        order.ApplyCoupon(
            couponId,
            "SAVE10",
            100);

        order.CouponId.Should().Be(couponId);
        order.CouponCode.Should().Be("SAVE10");
        order.DiscountAmount.Should().Be(100);
    }

    [Fact]
    public void Total_Should_Not_Be_Negative()
    {
        var order = CreateOrder();

        order.SetCharges(
            taxAmount: 0,
            shippingAmount: 0,
            discountAmount: 500);

        order.TotalAmount.Should().Be(0);
    }

}