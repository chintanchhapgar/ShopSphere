namespace ShopSphere.Application.Features.Products.Common;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string SKU,
    decimal BasePrice,
    decimal? CostPrice,
    Guid CategoryId,
    string CategoryName,
    Guid BrandId,
    string BrandName,
    bool IsActive,
    string? PrimaryImageUrl);