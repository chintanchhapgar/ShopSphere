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
            .Include(x => x.Inventory)
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

    public async Task<bool> AddOrRestoreAsync(
    Product product,
    CancellationToken cancellationToken)
    {
        var existing = await Context.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.SKU == product.SKU,
                cancellationToken);

        if (existing is null)
        {
            await AddAsync(
                product,
                cancellationToken);

            return true;
        }

        if (!existing.IsDeleted)
        {
            return false;
        }

        existing.Restore();

        existing.Update(
            product.Name,
            product.Description,
            product.SKU,
            product.BasePrice,
            product.CostPrice,
            product.CategoryId,
            product.BrandId,
            product.Slug,
            product.Barcode,
            product.Weight);

        return true;
    }
}