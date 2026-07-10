using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Common.Models;
using ShopSphere.Application.Features.Products.SearchProducts;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class ProductReadRepository
    : IProductReadRepository
{
    private readonly ApplicationDbContext _context;

    public ProductReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductSearchResponse>> SearchAsync(
    ProductSearchRequest request,
    CancellationToken cancellationToken)
    {
        var query =
            from product in _context.Products.AsNoTracking()
            join category in _context.Categories
                on product.CategoryId equals category.Id
            join brand in _context.Brands
                on product.BrandId equals brand.Id
            join inventory in _context.Inventories
                on product.Id equals inventory.ProductId into inventories
            from inventory in inventories.DefaultIfEmpty()
            select new
            {
                Product = product,
                CategoryName = category.Name,
                BrandName = brand.Name,
                Stock = inventory == null
                    ? 0
                    : inventory.AvailableQuantity
            };

        // Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                EF.Functions.Like(x.Product.Name, $"%{search}%") ||
                EF.Functions.Like(x.Product.Description, $"%{search}%") ||
                EF.Functions.Like(x.Product.SKU, $"%{search}%"));
        }

        // Category
        if (request.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.Product.CategoryId == request.CategoryId.Value);
        }

        // Brand
        if (request.BrandId.HasValue)
        {
            query = query.Where(x =>
                x.Product.BrandId == request.BrandId.Value);
        }

        // Price
        if (request.MinPrice.HasValue)
        {
            query = query.Where(x =>
                x.Product.BasePrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x =>
                x.Product.BasePrice <= request.MaxPrice.Value);
        }

        // Featured
        if (request.Featured.HasValue)
        {
            query = query.Where(x =>
                x.Product.IsFeatured == request.Featured.Value);
        }

        // Stock
        if (request.InStock.HasValue)
        {
            query = request.InStock.Value
                ? query.Where(x => x.Stock > 0)
                : query.Where(x => x.Stock <= 0);
        }

        // Sorting
        query = request.SortBy switch
        {
            ProductSortBy.Name =>
                query.OrderBy(x => x.Product.Name),

            ProductSortBy.NameDesc =>
                query.OrderByDescending(x => x.Product.Name),

            ProductSortBy.PriceAsc =>
                query.OrderBy(x => x.Product.BasePrice),

            ProductSortBy.PriceDesc =>
                query.OrderByDescending(x => x.Product.BasePrice),

            ProductSortBy.Oldest =>
                query.OrderBy(x => x.Product.CreatedAtUtc),

            _ =>
                query.OrderByDescending(x => x.Product.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(
            cancellationToken);

        var products = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ProductSearchResponse(
                x.Product.Id,
                x.Product.Name,
                x.Product.SKU,
                x.Product.BasePrice,
                x.CategoryName,
                x.BrandName,
                x.Stock,
                x.Product.IsFeatured,

                _context.Reviews
                    .Where(r =>
                        r.ProductId == x.Product.Id &&
                        r.Status == Domain.Enums.ReviewStatus.Approved)
                    .Select(r => (decimal?)r.Rating)
                    .Average() ?? 0,

                _context.Reviews
                    .Count(r =>
                        r.ProductId == x.Product.Id &&
                        r.Status == Domain.Enums.ReviewStatus.Approved),

                _context.ProductImages
                    .Where(i => i.ProductId == x.Product.Id)
                    .OrderBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductSearchResponse>(
            products,
            request.Page,
            request.PageSize,
            totalCount);
    }
}