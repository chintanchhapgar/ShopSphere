using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class WishlistErrors
{
    public static readonly Error AlreadyExists =
        Error.Conflict(
            "Wishlist.AlreadyExists",
            "Product already exists in wishlist.");

    public static readonly Error NotFound =
    Error.NotFound(
        "Wishlist.NotFound",
        "Product not found in wishlist.");
}