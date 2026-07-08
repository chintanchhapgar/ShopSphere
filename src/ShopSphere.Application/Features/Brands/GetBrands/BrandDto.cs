namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed record BrandDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive);