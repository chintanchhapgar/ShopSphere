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