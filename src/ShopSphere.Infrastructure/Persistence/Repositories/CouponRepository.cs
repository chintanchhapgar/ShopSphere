using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;
using System;

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

    public async Task<bool> AddOrRestoreAsync(
    Coupon coupon,
    CancellationToken cancellationToken)
    {
        var existing = await _context.Coupons
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Code == coupon.Code,
                cancellationToken);

        if (existing is null)
        {
            await AddAsync(
                coupon,
                cancellationToken);

            return true;
        }

        if (!existing.IsDeleted)
        {
            return false;
        }

        existing.Restore();

        existing.Update(
           coupon.Code,
           coupon.Name,
           coupon.Description,
           coupon.DiscountType,
           coupon.DiscountValue,
           coupon.MinimumOrderAmount,
           coupon.MaximumDiscountAmount,
           coupon.StartsAtUtc,
           coupon.ExpiresAtUtc,
           coupon.UsageLimit,
           coupon.IsActive);

        return true;
    }
}