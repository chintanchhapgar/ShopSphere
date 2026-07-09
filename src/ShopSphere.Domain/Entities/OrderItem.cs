

namespace ShopSphere.Domain.Entities;

public sealed class OrderItem : Entity
{
    private OrderItem() { }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = default!;

    public string SKU { get; private set; } = default!;

    public string? ImageUrl { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice { get; private set; }

    public Order Order { get; private set; } = default!;

    public static OrderItem Create(
        Guid productId,
        string productName,
        string sku,
        string? imageUrl,
        decimal unitPrice,
        int quantity)
    {
        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            SKU = sku,
            ImageUrl = imageUrl,
            UnitPrice = unitPrice,
            Quantity = quantity,
            TotalPrice = unitPrice * quantity
        };
    }
}