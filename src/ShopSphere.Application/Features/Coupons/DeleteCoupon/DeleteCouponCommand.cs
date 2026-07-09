using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Coupons.DeleteCoupon;

public sealed record DeleteCouponCommand(
    Guid Id)
    : IRequest<Result>;