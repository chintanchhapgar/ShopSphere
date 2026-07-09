using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Coupons.GetCoupons;

public sealed record CouponDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    DateTime StartsAtUtc,
    DateTime ExpiresAtUtc,
    int UsageLimit,
    int UsedCount,
    bool IsActive);