namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed record BrandResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);