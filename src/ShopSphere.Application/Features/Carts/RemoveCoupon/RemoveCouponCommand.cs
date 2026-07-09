using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.RemoveCoupon;

public sealed record RemoveCouponCommand
    : IRequest<Result>;