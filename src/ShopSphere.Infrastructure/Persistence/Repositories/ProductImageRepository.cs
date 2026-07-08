using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;
using ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ProductImageRepository
    : Repository<ProductImage>,
      IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(
        ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        Guid productId,
    CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.DisplayOrder)
            .ThenByDescending(x => x.IsPrimary)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task ClearPrimaryImageAsync(
    Guid productId,
    CancellationToken cancellationToken)
    {
        var images = await _context.ProductImages
            .Where(x => x.ProductId == productId &&
                        x.IsPrimary)
            .ToListAsync(cancellationToken);

        foreach (var image in images)
        {
            image.RemovePrimary();
        }
    }
}