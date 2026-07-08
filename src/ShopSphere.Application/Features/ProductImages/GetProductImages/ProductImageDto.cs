namespace ShopSphere.Application.Features.ProductImages.GetProductImages;

public sealed record ProductImageDto(
    Guid Id,
    string ImageUrl,
    int DisplayOrder,
    bool IsPrimary);