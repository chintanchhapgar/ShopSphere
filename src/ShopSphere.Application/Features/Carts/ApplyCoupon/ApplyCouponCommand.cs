using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.ApplyCoupon;

public sealed record ApplyCouponCommand(
    string CouponCode)
    : IRequest<Result>;