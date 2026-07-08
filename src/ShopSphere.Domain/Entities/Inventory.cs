namespace ShopSphere.Domain.Entities;

public sealed class Inventory : AuditableEntity
{
    private Inventory()
    {
    }

    public Inventory(
        Guid productId,
        int quantityOnHand,
        int lowStockThreshold)
    {
        ProductId = productId;
        QuantityOnHand = quantityOnHand;
        LowStockThreshold = lowStockThreshold;
    }

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int QuantityOnHand { get; private set; }

    public int ReservedQuantity { get; private set; }

    public int LowStockThreshold { get; private set; }

    public int AvailableQuantity =>
        QuantityOnHand - ReservedQuantity;

    public bool IsLowStock =>
        AvailableQuantity <= LowStockThreshold;

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        QuantityOnHand += quantity;
    }
    public void AdjustStock(int quantity)
    {
        if (quantity > 0)
        {
            IncreaseStock(quantity);
            return;
        }

        DecreaseStock(Math.Abs(quantity));
    }
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        QuantityOnHand -= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        ReservedQuantity += quantity;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException("Reserved quantity is too low.");

        ReservedQuantity -= quantity;
    }

    public void SetLowStockThreshold(int threshold)
    {
        if (threshold < 0)
            throw new ArgumentOutOfRangeException(nameof(threshold));

        LowStockThreshold = threshold;
    }
}