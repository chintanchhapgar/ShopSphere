

namespace ShopSphere.Domain.Entities;

public sealed class ProductImage : AuditableEntity
{
    private ProductImage()
    {
    }

    public ProductImage(
        Guid productId,
        string imageUrl,
        int displayOrder,
        bool isPrimary)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
    }

    public Guid ProductId { get; private set; }

    public string ImageUrl { get; private set; } = default!;

    public int DisplayOrder { get; private set; }

    public bool IsPrimary { get; private set; }

    public Product Product { get; private set; } = default!;

    public void SetPrimary()
    {
        IsPrimary = true;
    }

    public void RemovePrimary()
    {
        IsPrimary = false;
    }

    public void ChangeDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

    public void UpdateImage(string imageUrl)
    {
        ImageUrl = imageUrl;
    }

    public void SetDisplayOrder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }

}