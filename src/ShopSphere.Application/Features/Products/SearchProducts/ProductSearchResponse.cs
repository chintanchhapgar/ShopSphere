namespace ShopSphere.Application.Features.Products.SearchProducts;

public sealed record ProductSearchResponse(
    Guid Id,
    string Name,
    string SKU,
    decimal Price,
    string Category,
    string Brand,
    int Stock,
    bool IsFeatured,
    decimal AverageRating,
    int TotalReviews,
    string? Thumbnail);