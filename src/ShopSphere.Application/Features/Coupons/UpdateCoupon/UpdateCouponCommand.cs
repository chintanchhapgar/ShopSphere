using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Coupons.UpdateCoupon;

public sealed record UpdateCouponCommand(
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
    bool IsActive)
    : IRequest<Result>;