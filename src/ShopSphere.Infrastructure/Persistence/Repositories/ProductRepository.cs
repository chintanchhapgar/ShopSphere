using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository
    : Repository<Product>, IProductRepository
{
    public ProductRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public override async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await Entities
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await Entities
            .Include(x => x.Category)
            .Include(x => x.Brand)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludeId,
        CancellationToken cancellationToken)
    {
        sku = sku.Trim().ToUpperInvariant();

        return await Entities.AnyAsync(
            x => x.SKU == sku &&
                 x.Id != excludeId,
            cancellationToken);
    }
}