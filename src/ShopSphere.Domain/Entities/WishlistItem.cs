namespace ShopSphere.Domain.Entities;

public sealed class WishlistItem : AuditableEntity
{
    private WishlistItem()
    {
    }

    public Guid WishlistId { get; private set; }

    public Wishlist Wishlist { get; private set; } = default!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = default!;

    public static WishlistItem Create(
        Guid productId)
    {
        return new WishlistItem
        {
            ProductId = productId
        };
    }
}