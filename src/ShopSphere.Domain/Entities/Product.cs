

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

    public string Slug { get; private set; } = default!;

    public string? Barcode { get; private set; }

    public decimal? Weight { get; private set; }

    public bool IsFeatured { get; private set; }

    private readonly List<ProductImage> _images = [];

    public IReadOnlyCollection<ProductImage> Images
        => _images.AsReadOnly();

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
        Guid brandId,
        string slug,
        string? barcode,
        decimal? weight)
    {
        Name = name.Trim();
        Description = description.Trim();
        SKU = sku.Trim().ToUpperInvariant();
        BasePrice = basePrice;
        CostPrice = costPrice;
        CategoryId = categoryId;
        BrandId = brandId;
        Slug = slug;
        Barcode = barcode;
        Weight = weight;
    }

    public void SetFeatured(bool featured)
    {
        IsFeatured = featured;
    }

    public void Update(
        string name,
        string description,
        string sku,
        decimal basePrice,
        decimal? costPrice,
        Guid categoryId,
        Guid brandId,
        string slug,
        string? barcode,
        decimal? weight)
    {
        Name = name;
        Description = description;
        SKU = sku.ToUpperInvariant();
        BasePrice = basePrice;
        CostPrice = costPrice;
        CategoryId = categoryId;
        BrandId = brandId;
        Slug = slug;
        Barcode = barcode;
        Weight = weight;
    }

}