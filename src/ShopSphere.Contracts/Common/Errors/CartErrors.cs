using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class CartErrors
{
    public static readonly Error NotFound = new(
        "Cart.NotFound",
        "Cart not found.");

    public static readonly Error Empty = new(
        "Cart.Empty",
        "Your cart is empty.");

    public static readonly Error ItemNotFound = new(
        "Cart.ItemNotFound",
        "Cart item not found.");

    public static readonly Error ProductAlreadyExists = new(
        "Cart.ProductAlreadyExists",
        "Product already exists in the cart.");

    public static readonly Error InvalidQuantity = new(
        "Cart.InvalidQuantity",
        "Invalid quantity.");

    public static readonly Error Unauthorized = new(
        "Cart.Unauthorized",
        "You are not authorized to access this cart.");

    public static readonly Error CouponNotApplied = new(
        "Cart.CouponNotApplied",
        "No coupon is applied to the cart.");
}