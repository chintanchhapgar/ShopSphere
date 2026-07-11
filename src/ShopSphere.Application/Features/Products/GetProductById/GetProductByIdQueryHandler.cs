using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Products.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductQueries _queries;
    private readonly ICacheService _cacheService;

    public GetProductByIdQueryHandler(
        IProductQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"products:{request.Id}";

        var cachedProduct = await _cacheService.GetAsync<ProductDto>(
            cacheKey,
            cancellationToken);

        if (cachedProduct is not null)
        {
            return Result<ProductDto>.Success(
                cachedProduct,
                "Product retrieved from cache.");
        }

        var product = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result<ProductDto>.Failure(
                ProductErrors.NotFound);
        }

        await _cacheService.SetAsync(
            cacheKey,
            product,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<ProductDto>.Success(
            product,
            "Product retrieved successfully.");
    }
}