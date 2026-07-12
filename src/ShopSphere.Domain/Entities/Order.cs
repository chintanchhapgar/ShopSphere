
using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class Order : AuditableEntity
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    public Guid UserId { get; private set; }

    public string OrderNumber { get; private set; } = default!;

    public OrderStatus Status { get; private set; }

    public decimal SubTotal { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal ShippingAmount { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public string ShippingName { get; private set; } = default!;

    public string PhoneNumber { get; private set; } = default!;

    public string AddressLine1 { get; private set; } = default!;

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; } = default!;

    public string State { get; private set; } = default!;

    public string PostalCode { get; private set; } = default!;

    public string Country { get; private set; } = default!;

    public Guid? CouponId { get; private set; }
    public string? CouponCode { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Payment? Payment { get; private set; }
    public Shipment? Shipment { get; private set; }
    public static Order Create(
        Guid userId,
        string orderNumber,
        string shippingName,
        string phoneNumber,
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string postalCode,
        string country)
    {
        return new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            Status = OrderStatus.Pending,
            ShippingName = shippingName,
            PhoneNumber = phoneNumber,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country
        };
    }

    public void AddItem(OrderItem item)
    {
        _items.Add(item);
        RecalculateTotals();
    }

    public void SetCharges(
        decimal taxAmount,
        decimal shippingAmount,
        decimal discountAmount)
    {
        TaxAmount = taxAmount;
        ShippingAmount = shippingAmount;
        DiscountAmount = discountAmount;

        RecalculateTotals();
    }

    public void SetCharges(
    decimal taxAmount,
    decimal shippingAmount)
    {
        TaxAmount = taxAmount;
        ShippingAmount = shippingAmount;

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(x => x.TotalPrice);

        TotalAmount =
        Math.Max(
            0,
            SubTotal +
            TaxAmount +
            ShippingAmount -
            DiscountAmount);
    }

    public void ApplyCoupon(
    Guid couponId,
    string couponCode,
    decimal discountAmount)
    {
        CouponId = couponId;
        CouponCode = couponCode;
        DiscountAmount = discountAmount;

        RecalculateTotals();
    }
    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }

    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Only confirmed orders can be processed.");
        }

        Status = OrderStatus.Processing;
    }

    public void MarkShipped()
    {
        if (Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException(
                "Only processing orders can be shipped.");
        }

        Status = OrderStatus.Shipped;
    }

    public void MarkDelivered()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new InvalidOperationException(
                "Only shipped orders can be delivered.");
        }

        Status = OrderStatus.Delivered;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                "Only delivered orders can be completed.");
        }

        Status = OrderStatus.Completed;
    }

    public bool CanBeCompleted()
    {
        return Status == OrderStatus.Delivered;
    }

    private bool CanMoveTo(OrderStatus status)
    {
        return Status switch
        {
            OrderStatus.Pending =>
                status == OrderStatus.Confirmed ||
                status == OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                status == OrderStatus.Processing,

            OrderStatus.Processing =>
                status == OrderStatus.Shipped,

            OrderStatus.Shipped =>
                status == OrderStatus.Delivered,

            OrderStatus.Delivered =>
                false,

            OrderStatus.Cancelled =>
                false,

            _ => false
        };
    }
}