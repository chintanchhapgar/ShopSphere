using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Application.Features.Coupons.GetCoupons;

namespace ShopSphere.Application.Features.Coupons.GetCouponById;

public sealed record GetCouponByIdQuery(
    Guid Id)
    : IRequest<Result<CouponDto>>;