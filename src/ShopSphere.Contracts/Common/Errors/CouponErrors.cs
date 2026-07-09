using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class CouponErrors
{
    public static readonly Error NotFound = new(
        "Coupon.NotFound",
        "Coupon not found.");

    public static readonly Error AlreadyExists = new(
        "Coupon.AlreadyExists",
        "Coupon code already exists.");

    public static readonly Error Expired = new(
        "Coupon.Expired",
        "Coupon has expired.");

    public static readonly Error UsageLimitReached = new(
        "Coupon.UsageLimitReached",
        "Coupon usage limit has been reached.");

    public static readonly Error Invalid = new(
        "Coupon.Invalid",
        "Coupon is invalid.");
}