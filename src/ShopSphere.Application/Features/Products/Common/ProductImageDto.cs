namespace ShopSphere.Application.Features.Products.Common;

public sealed record ProductImageDto(
    Guid Id,
    string ImageUrl,
    int DisplayOrder,
    bool IsPrimary);