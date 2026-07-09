
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

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Payment? Payment { get; private set; }

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

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(x => x.TotalPrice);

        TotalAmount =
            SubTotal +
            TaxAmount +
            ShippingAmount -
            DiscountAmount;
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

    public void UpdateStatus(OrderStatus status)
    {
        if (!CanMoveTo(status))
        {
            throw new InvalidOperationException(
                $"Cannot change order status from {Status} to {status}.");
        }

        Status = status;
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