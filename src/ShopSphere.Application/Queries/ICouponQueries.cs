using ShopSphere.Application.Features.Coupons.GetCoupons;

namespace ShopSphere.Application.Queries;

public interface ICouponQueries
{
    Task<IReadOnlyList<CouponDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CouponDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}