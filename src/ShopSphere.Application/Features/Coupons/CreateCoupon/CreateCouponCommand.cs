using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Coupons.CreateCoupon;

public sealed record CreateCouponCommand(
    string Code,
    string Name,
    string? Description,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    DateTime StartsAtUtc,
    DateTime ExpiresAtUtc,
    int UsageLimit)
    : IRequest<Result<Guid>>;