namespace ShopSphere.Application.Features.Wishlists.GetWishlist;

public sealed record WishlistItemResponse(
    Guid ProductId,
    string Name,
    string SKU,
    decimal Price,
    string? ImageUrl,
    bool InStock);