using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class ProductQueries : IProductQueries
{
    private readonly ApplicationDbContext _context;

    public ProductQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Description,
                x.SKU,
                x.BasePrice,
                x.CostPrice,
                x.CategoryId,
                x.Category.Name,
                x.BrandId,
                x.Brand.Name,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Description,
                x.SKU,
                x.BasePrice,
                x.CostPrice,
                x.CategoryId,
                x.Category.Name,
                x.BrandId,
                x.Brand.Name,
                x.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }
}