using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed class GetBrandsQueryHandler
    : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandDto>>>
{
    private readonly IBrandQueries _queries;
    private readonly ICacheService _cacheService;
    public GetBrandsQueryHandler(
        IBrandQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<IReadOnlyList<BrandDto>>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {

        const string cacheKey = "brands:list";

        var cached = await _cacheService.GetAsync<IReadOnlyList<BrandDto>>(
            cacheKey,
            cancellationToken);

        if (cached is not null)
        {
            return Result<IReadOnlyList<BrandDto>>.Success(
                cached,
                "Brands retrieved from cache.");
        }

        var brands = await _queries.GetAllAsync(cancellationToken);

        await _cacheService.SetAsync(
            cacheKey,
            brands,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<IReadOnlyList<BrandDto>>
            .Success(brands, "Brands retrieved successfully.");
    }
}