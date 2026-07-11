using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategories;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly ICategoryQueries _queries;
    private readonly ICacheService _cacheService;
    public GetCategoriesQueryHandler(
        ICategoryQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "categories:list";

        var cached = await _cacheService.GetAsync<IReadOnlyList<CategoryDto>>(
           cacheKey,
           cancellationToken);

        if (cached is not null)
        {
            return Result<IReadOnlyList<CategoryDto>>.Success(
                cached,
                "Categories retrieved from cache.");
        }

        var categories = await _queries.GetAllAsync(cancellationToken);

        await _cacheService.SetAsync(
            cacheKey,
            categories,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<IReadOnlyList<CategoryDto>>
            .Success(categories, "Categories retrieved successfully.");
    }
}