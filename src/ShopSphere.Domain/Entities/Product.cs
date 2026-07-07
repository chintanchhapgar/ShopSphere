using ShopSphere.Domain.Common;

namespace ShopSphere.Domain.Entities;

public sealed class Product : AuditableEntity
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public string SKU { get; private set; }

    public decimal BasePrice { get; private set; }

    public decimal? CostPrice { get; private set; }

    public Guid CategoryId { get; private set; }

    public Guid BrandId { get; private set; }

    public Category Category { get; private set; } = null!;

    public Brand Brand { get; private set; } = null!;

    private Product()
    {
    }

    public Product(
        string name,
        string description,
        string sku,
        decimal basePrice,
        decimal? costPrice,
        Guid categoryId,
        Guid brandId)
    {
        Name = name.Trim();
        Description = description.Trim();
        SKU = sku.Trim().ToUpperInvariant();
        BasePrice = basePrice;
        CostPrice = costPrice;
        CategoryId = categoryId;
        BrandId = brandId;
    }

    public void Update(
        string name,
        string description,
        string sku,
        decimal basePrice,
        decimal? costPrice,
        Guid categoryId,
        Guid brandId)
    {
        Name = name.Trim();
        Description = description.Trim();
        SKU = sku.Trim().ToUpperInvariant();
        BasePrice = basePrice;
        CostPrice = costPrice;
        CategoryId = categoryId;
        BrandId = brandId;
    }

}