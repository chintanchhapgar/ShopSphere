using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Coupons.GetCoupons;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class CouponQueries : ICouponQueries
{
    private readonly ApplicationDbContext _context;

    public CouponQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CouponDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Coupons
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CouponDto(
                x.Id,
                x.Code,
                x.Name,
                x.Description,
                x.DiscountType,
                x.DiscountValue,
                x.MinimumOrderAmount,
                x.MaximumDiscountAmount,
                x.StartsAtUtc,
                x.ExpiresAtUtc,
                x.UsageLimit,
                x.UsedCount,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<CouponDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Coupons
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CouponDto(
                x.Id,
                x.Code,
                x.Name,
                x.Description,
                x.DiscountType,
                x.DiscountValue,
                x.MinimumOrderAmount,
                x.MaximumDiscountAmount,
                x.StartsAtUtc,
                x.ExpiresAtUtc,
                x.UsageLimit,
                x.UsedCount,
                x.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }
}