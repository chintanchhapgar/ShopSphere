using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class BrandQueries : IBrandQueries
{
    private readonly ApplicationDbContext _context;

    public BrandQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BrandDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new BrandDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<BrandDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Brands
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new BrandDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }
}