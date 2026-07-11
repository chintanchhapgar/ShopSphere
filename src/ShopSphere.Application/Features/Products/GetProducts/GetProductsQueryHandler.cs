using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.GetProducts;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IProductQueries _queries;
    private readonly ICacheService _cacheService;
    public GetProductsQueryHandler(
        IProductQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {

        const string cacheKey = "products:list";

        var cached = await _cacheService.GetAsync<IReadOnlyList<ProductDto>>(
            cacheKey,
            cancellationToken);

        if (cached is not null)
        {
            return Result<IReadOnlyList<ProductDto>>.Success(
                cached,
                "Products retrieved from cache.");
        }

        var products = await _queries.GetAllAsync(cancellationToken);

        await _cacheService.SetAsync(
            cacheKey,
            products,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<IReadOnlyList<ProductDto>>
             .Success(products, "Products retrieved successfully.");
    }
}