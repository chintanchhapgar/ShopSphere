using MediatR;
using ShopSphere.Application.Features.Coupons.GetCoupons;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Coupons.GetCouponById;

public sealed class GetCouponByIdQueryHandler
    : IRequestHandler<GetCouponByIdQuery, Result<CouponDto>>
{
    private readonly ICouponQueries _couponQueries;

    public GetCouponByIdQueryHandler(
        ICouponQueries couponQueries)
    {
        _couponQueries = couponQueries;
    }

    public async Task<Result<CouponDto>> Handle(
        GetCouponByIdQuery request,
        CancellationToken cancellationToken)
    {
        var coupon = await _couponQueries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (coupon is null)
        {
            return Result<CouponDto>.Failure(
                CouponErrors.NotFound);
        }

        return Result<CouponDto>.Success(coupon);
    }
}