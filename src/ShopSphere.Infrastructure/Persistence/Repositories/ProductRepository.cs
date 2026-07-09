using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository
    : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(
        ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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