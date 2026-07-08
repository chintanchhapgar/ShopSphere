using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Common;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

public sealed class ProductQueries : IProductQueries
{
    private readonly ApplicationDbContext _context;

    public ProductQueries(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailsDto?> GetDetailsAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new ProductDetailsDto(
                x.Id,
                x.Name,
                x.Description,
                x.SKU,
                x.BasePrice,
                x.CostPrice,
                x.IsActive,
                new LookupDto(
                    x.Category.Id,
                    x.Category.Name),
                new LookupDto(
                    x.Brand.Id,
                    x.Brand.Name),
                x.Images
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => new ProductImageDto(
                        i.Id,
                        i.ImageUrl,
                        i.DisplayOrder,
                        i.IsPrimary))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
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
                x.IsActive,
                x.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()))
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
                x.IsActive,
                x.Images
                    .Where(i => i.IsPrimary)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}