using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryQueries _queries;
    private readonly ICacheService _cacheService;

    public GetCategoryByIdQueryHandler(
        ICategoryQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {

        var cacheKey = $"categories:{request.Id}";

        var cachedCategory = await _cacheService.GetAsync<CategoryDto>(
            cacheKey,
            cancellationToken);

        if (cachedCategory is not null)
        {
            return Result<CategoryDto>.Success(
                cachedCategory,
                "Category retrieved from cache.");
        }

        var category = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure(
                CategoryErrors.NotFound);
        }

        await _cacheService.SetAsync(
            cacheKey,
            category,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<CategoryDto>
           .Success(category, "Category retrieved successfully.");
    }
}