namespace ShopSphere.Domain.Entities;

public sealed class Cart : AuditableEntity
{
    private readonly List<CartItem> _items = [];

    private Cart()
    {
    }

    public Cart(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid CustomerId { get; private set; }


    public IReadOnlyCollection<CartItem> Items =>
        _items.AsReadOnly();

    public decimal Total =>
        _items.Sum(x => x.Subtotal);

    public void AddItem(
        Guid productId,
        int quantity,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        var existing = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            return;
        }

        _items.Add(new CartItem(
            productId,
            quantity,
            unitPrice));
    }

    public void UpdateQuantity(
        Guid itemId,
        int quantity)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId)
            ?? throw new InvalidOperationException("Cart item not found.");

        item.UpdateQuantity(quantity);
    }

    public CartItem? RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return null;
        _items.Remove(item);
        return item; // caller decides what to do with it
    }

    public void Clear()
    {
        _items.Clear();
    }
}