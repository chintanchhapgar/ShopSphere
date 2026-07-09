using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Coupons.GetCoupons;

public sealed record GetCouponsQuery
    : IRequest<Result<IReadOnlyList<CouponDto>>>;