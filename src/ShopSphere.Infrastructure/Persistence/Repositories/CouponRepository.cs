using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _context;

    public CouponRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Coupon?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Coupon?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken)
    {
        return await _context.Coupons
            .FirstOrDefaultAsync(
                x => x.Code == code.ToUpper(),
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        return await _context.Coupons.AnyAsync(
            x => x.Code == code.ToUpper() &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task AddAsync(
        Coupon coupon,
        CancellationToken cancellationToken)
    {
        await _context.Coupons.AddAsync(
            coupon,
            cancellationToken);
    }

    public void Remove(
        Coupon coupon)
    {
        _context.Coupons.Remove(coupon);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(
            cancellationToken);
    }
}