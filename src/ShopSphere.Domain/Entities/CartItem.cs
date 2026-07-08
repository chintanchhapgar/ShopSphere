namespace ShopSphere.Domain.Entities;

public sealed class CartItem : Entity
{
    private CartItem()
    {
    }

    public CartItem(
        Guid cartId,
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid CartId { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Subtotal =>
        Quantity * UnitPrice;

    internal void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }

    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Quantity = quantity;
    }
}