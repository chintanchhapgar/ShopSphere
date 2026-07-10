namespace ShopSphere.Application.Features.Wishlists.GetWishlist;

public sealed record WishlistResponse(
    IReadOnlyCollection<WishlistItemResponse> Items);