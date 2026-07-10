namespace ShopSphere.Domain.Entities;

public sealed class Wishlist : AuditableEntity
{
    private readonly List<WishlistItem> _items = [];

    private Wishlist()
    {
    }

    public Guid CustomerId { get; private set; }

    public IReadOnlyCollection<WishlistItem> Items =>
        _items.AsReadOnly();

    public static Wishlist Create(
        Guid customerId)
    {
        return new Wishlist
        {
            CustomerId = customerId
        };
    }

    public void AddItem(
        Guid productId)
    {
        if (_items.Any(x => x.ProductId == productId))
        {
            return;
        }

        _items.Add(
            WishlistItem.Create(productId));
    }

    public void RemoveItem(
        Guid productId)
    {
        var item = _items.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is not null)
        {
            _items.Remove(item);
        }
    }

    public bool Contains(
        Guid productId)
    {
        return _items.Any(
            x => x.ProductId == productId);
    }

    public void Clear()
    {
        _items.Clear();
    }
}