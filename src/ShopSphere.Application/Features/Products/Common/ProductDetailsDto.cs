using ShopSphere.Application.Common;

namespace ShopSphere.Application.Features.Products.Common;

public sealed record ProductDetailsDto( 
    Guid Id,
    string Name,
    string Description,
    string SKU,
    decimal BasePrice,
    decimal? CostPrice,
    bool IsActive,
    LookupDto Category,
    LookupDto Brand,
    IReadOnlyList<ProductImageDto> Images);