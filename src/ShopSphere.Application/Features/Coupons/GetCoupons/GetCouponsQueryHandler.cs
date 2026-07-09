using MediatR;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Coupons.GetCoupons;

public sealed class GetCouponsQueryHandler
    : IRequestHandler<GetCouponsQuery, Result<IReadOnlyList<CouponDto>>>
{
    private readonly ICouponQueries _couponQueries;

    public GetCouponsQueryHandler(
        ICouponQueries couponQueries)
    {
        _couponQueries = couponQueries;
    }

    public async Task<Result<IReadOnlyList<CouponDto>>> Handle(
        GetCouponsQuery request,
        CancellationToken cancellationToken)
    {
        var coupons = await _couponQueries.GetAllAsync(
            cancellationToken);

        return Result<IReadOnlyList<CouponDto>>.Success(
            coupons);
    }
}