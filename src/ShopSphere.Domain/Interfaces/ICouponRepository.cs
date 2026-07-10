using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface ICouponRepository
{
    Task<Coupon?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Coupon coupon,
        CancellationToken cancellationToken);

    void Remove(
        Coupon coupon);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<bool> AddOrRestoreAsync(
        Coupon coupon,
        CancellationToken cancellationToken);
}